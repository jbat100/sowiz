import os.path
import argparse
import time
import logging

from sowiz.util import perform_logging_setup
from sowiz.description.config import AnnotationTypes
from sowiz.description.package import Package
from sowiz.description.core import get_event_file_reader_classes_for_extension
from sowiz.description.annotation import AnnotationOSCTranslator
from sowiz.description.midi import MidiOSCTranslator
from sowiz.description.player import EventPlayer, EventPrintClient, EventMultiClient, EventOSCClient

def osc_path_for_annotation_type(annotation_type):
	return '/sowiz/annotation/' + annotation_type.lower()

def main():

	parser = argparse.ArgumentParser(description='Test player for an description package')
	parser.add_argument('path', type=str, help='Path to the input package')
	args = parser.parse_args()
	perform_logging_setup(logging.DEBUG)
	package = Package(args.path)

	osc_client = EventOSCClient('localhost', 3333)

	annotation_translator = AnnotationOSCTranslator()
	for annotation_type in AnnotationTypes:
		annotation_translator.set_route(annotation_type, osc_path_for_annotation_type(annotation_type))
	osc_client.register_translator(annotation_translator)
	osc_client.register_translator(MidiOSCTranslator())

	client = EventMultiClient( [osc_client, EventPrintClient()] )

	player = EventPlayer(client)
	for file_path in package.file_paths:
		extension = os.path.splitext(file_path)[1]
		for cls in get_event_file_reader_classes_for_extension(extension):
			reader = cls(None, file_path)
			player.add_reader(reader)

	player.play()

	try:
		while True:
			time.sleep(100)
	except (KeyboardInterrupt, SystemExit), e:
		player.stop()

if __name__ == '__main__':
	main()
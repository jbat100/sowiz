import argparse
import time
import logging

from sowiz.util import perform_logging_setup
from sowiz.annotation.config import AnnotationTypes
from sowiz.annotation.package import Package
from sowiz.annotation.reader import AnnotationFileReader
from sowiz.annotation.player import AnnotationPlayer, AnnotationPrintClient, AnnotationMultiClient, AnnotationOSCClient

def osc_path_for_annotation_type(annotation_type):
	return '/sowiz/annotation/' + annotation_type.lower()

def main():

	parser = argparse.ArgumentParser(description='Test player for an annotation package')
	parser.add_argument('path', type=str, help='Path to the input package')
	args = parser.parse_args()

	perform_logging_setup(logging.INFO)

	package = Package(args.path)

	osc_client = AnnotationOSCClient('localhost', 3333)
	for annotation_type in AnnotationTypes:
		osc_client.set_route(annotation_type, osc_path_for_annotation_type(annotation_type))

	client = AnnotationMultiClient()
	client.add_client(AnnotationPrintClient())
	client.add_client(osc_client)

	player = AnnotationPlayer(client)
	for annotation_file_path in package.annotation_file_paths:
		reader = AnnotationFileReader(annotation_file_path)
		player.add_reader(reader)

	player.play()

	try:
		while True:
			time.sleep(100)
	except (KeyboardInterrupt, SystemExit), e:
		player.stop()

if __name__ == '__main__':
	main()
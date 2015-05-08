import os.path
import argparse

import sowiz.description.core
import sowiz.description.annotation
import sowiz.description.midi

if __name__ == '__main__':

	parser = argparse.ArgumentParser(description='Test an description package')
	parser.add_argument('input', type=str, help='Path to the input description file')
	args = parser.parse_args()

	ext = os.path.splitext( args.input )[1]

	for reader_cls in sowiz.description.core.get_all_event_file_reader_classes():

		if ext not in reader_cls.EXPECTED_EXTENSIONS:
			continue

		reader = reader_cls('test', args.input)

		for event in reader.events:
			print event
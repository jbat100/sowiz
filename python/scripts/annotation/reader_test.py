import argparse

from sowiz.annotation.reader import AnnotationFileReader

if __name__ == '__main__':

	parser = argparse.ArgumentParser(description='Test an annotation package')
	parser.add_argument('input', type=str, help='Path to the input annotation file')
	args = parser.parse_args()

	reader = AnnotationFileReader(args.input)

	for annotation in reader.annotations:
		print annotation
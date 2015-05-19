import argparse
import logging
import os.path
import numpy
import PIL.Image

from sowiz.util import perform_logging_setup

def main():

	channels = 'rgba'

	parser = argparse.ArgumentParser(description='Create an RGBA texture from four images (greyscales)')
	for chan in channels:
		parser.add_argument(chan, type=str, help='Path to the greyscale for %s channel' % chan.upper())
	parser.add_argument('--size', '-s', type=int, default=340, help='The size of the output')
	parser.add_argument('--out', '-o', type=str, default='output.png', help='Path of the output')
	#parser.add_argument('--intermediates', '-i', action=store_true, default='output.png', help='Path of the output')
	args = parser.parse_args()
	perform_logging_setup(logging.DEBUG)

	shape = (args.size, args.size, 4)

	output_image_array = numpy.zeros(shape)

	# open r, g, b, a images
	for i, chan in enumerate(channels):
		path = getattr(args, chan)
		logging.info('opening %s image from %s' % (chan, path))
		input_image = PIL.Image.open(path)
		resized_image = input_image.resize(shape[:2])
		greyscale_image = resized_image.convert('L')
		split_path = os.path.splitext(path)
		component_path = split_path[0] + '_' + chan + '_component.png'
		greyscale_image.save(component_path)
		greyscale_array = numpy.array(greyscale_image)
		output_image_array[..., i] = greyscale_array

	output_image = PIL.Image.fromarray(numpy.uint8(output_image_array))
	output_image.save(args.out)


if __name__ == '__main__':
	main()

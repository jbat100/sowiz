import argparse
import os

from sowiz.annotation.package import PackageGenerator

if __name__ == '__main__':

	annotations = ["beat_spectral_diff.n3", 'rms_amplitude.n3', 'chronogram.n3', 'spectral_centroid.n3', 'constant_q_spect.n3']

	parser = argparse.ArgumentParser(description='Generate an annotation package')
	parser.add_argument('input', type=str, help='Path to the input audio file')
	parser.add_argument('-o', '--output-directory', type=str, help='The output package directory')
	parser.add_argument('-z', '--zip', action='store_true', help='Zip the output directory')
	args = parser.parse_args()

	script_path_components = os.path.dirname(os.path.realpath(__file__)).split(os.sep)
	annotation_config_path_components = script_path_components[:-3] + ['config', 'annotation']
	annotation_config_path = os.sep.join(annotation_config_path_components)

	generator = PackageGenerator(args.input, args.output_directory)

	for annotation_path in [os.path.join(annotation_config_path, annotation) for annotation in annotations]:
		generator.add_transform_path(annotation_path)

	generator.generate()



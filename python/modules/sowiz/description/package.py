import os
import shutil
import subprocess


class Package(object):

	def __init__(self, package_path):
		self.__path = package_path

	@property
	def path(self):
		return self.__path

	@property
	def file_names(self):
		items = os.listdir(self.path)
		for item in items:
			yield item

	@property
	def file_paths(self):
	    for name in self.file_names:
			yield os.path.join(self.path, name)


class PackageGenerator(object):

	def __init__(self, input_path, output_path=None):

		self.__input_path = input_path
		self.__output_path = output_path
		self.__transform_paths = []
		self.__zip = False

	@property
	def input_path(self):

		return self.__input_path

	@property
	def output_path(self):

		if self.__output_path is not None:
			return self.__output_path
		else:
			return os.path.splitext(self.input_path)[0]

	@property
	def transform_paths(self):

		return iter(self.__transform_paths)

	def add_transform_path(self, transform_path):

		if transform_path not in self.__transform_paths:
			self.__transform_paths.append(transform_path)
		else:
			raise ValueError(u"transform path already exists: {0:s}".format(transform_path))

	@staticmethod
	def ensure_directory(directory_path):

		if not os.path.exists(directory_path):
			os.makedirs(directory_path)

	@staticmethod
	def check_file_path(file_path):

		if not os.path.isfile(file_path):
			raise RuntimeError(u"invalid file path : {0:s}".format(file_path))

	def generate(self):

		self.check_file_path(self.input_path)
		self.ensure_directory(self.output_path)

		_, input_name = os.path.split(self.input_path)
		# copy the input audio file into the output package
		packaged_input_path = os.path.join(self.output_path, input_name)
		shutil.copyfile(self.input_path, packaged_input_path)

		cmd_list = ['sonic-annotator']
		for transform_path in self.transform_paths:
			self.check_file_path(transform_path)
			_, transform_name = os.path.split(transform_path)
			# copy the transform into the output package
			shutil.copyfile(transform_path, os.path.join(self.output_path, transform_name))
			cmd_list += ['-t', transform_path]

		cmd_list += [packaged_input_path, '-w', 'csv', '--csv-force']

		p = subprocess.Popen(cmd_list)
		p.wait()
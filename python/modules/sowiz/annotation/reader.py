
import logging
import os
import time

from sowiz.util import StoppableThread
from sowiz.annotation.config import annotation_type_for_annotation_file_name

class Annotation(object):

	def __init__(self, identifier, time_stamp, values):
		self.__identifier = identifier
		self.__time_stamp = time_stamp
		self.__values = values

	def __str__(self):
		s = self.__class__.__name__ + ' ' + self.identifier + ' time_stamp: ' + str(self.time_stamp)
		s += ' values : ' + str(self.__values)
		return s

	@property
	def identifier(self):
	    return self.__identifier

	@property
	def time_stamp(self):
		return self.__time_stamp

	@property
	def values(self):
		return iter(self.__values)

	@classmethod
	def new_from_string(cls, identifier, annotation_string, seperator=','):
		all_values = annotation_string.split(seperator)
		if len(all_values) == 0:
			return None
		time_stamp = float(all_values[0])
		values = [float(v) for v in all_values[1:]]
		return cls(identifier, time_stamp, values)


class AnnotationFileReader(object):

	def __init__(self, file_path, identifier=None):
		self.__file_path = file_path
		if identifier is None:
			self.__identifier = annotation_type_for_annotation_file_name(os.path.split(file_path)[1])
		else:
			self.__identifier = identifier

	def __str__(self):
		return self.__class__.__name__ + ' ' + self.__file_path

	@property
	def file_path(self):
		return self.__file_path

	@property
	def identifier(self):
		# return the name of the file without extention
		return self.__identifier

	@property
	def annotations(self):
		with open(self.file_path, 'r') as f:
			for line in f:
				annotation = Annotation.new_from_string(self.identifier, line)
				assert annotation
				yield annotation



class AnnotationReaderThread(StoppableThread):

	def __init__(self, reader, queue):
		super(AnnotationReaderThread, self).__init__()
		self.__reader  = reader
		self.__queue = queue
		self.__start_time = None

	@property
	def reader(self):
		return self.__reader

	@property
	def queue(self):
		return self.__queue

	@property
	def start_time(self):
		return self.__start_time

	def run(self):
		logging.debug('starting reader thread %s' % str(self.reader))
		self.__start_time = time.time()
		for annotation in self.reader.annotations:
			wait_time = self.start_time + annotation.time_stamp - time.time()
			logging.debug('next annotation in %f seconds' % wait_time)
			if wait_time > 0.0:
				self.sleep(wait_time)
			if self.is_stopped():
				break
			self.queue.put(annotation)

import os

from sowiz.network.osc import Message
from sowiz.util import variable_type_check
from sowiz.description.core import Event, EventFileReader
from sowiz.description.config import annotation_type_for_annotation_file_name

class Annotation(Event):

	def __init__(self, identifier, time_stamp, values):
		super(Annotation, self).__init__(identifier, time_stamp)
		self.__values = values

	def __str__(self):
		s = super(Annotation, self).__str__()
		s += ' values : ' + str(self.__values)
		return s

	@property
	def values(self):
		return iter(self.__values)

	@classmethod
	def new_from_string(cls, identifier, annotation_string, seperator=','):
		all_values = annotation_string.split(seperator)
		if len(all_values) == 0:
			raise ValueError( 'cannot create annotation from string : %s' % annotation_string )
		time_stamp = float(all_values[0])
		values = [float(v) for v in all_values[1:]]
		return cls(identifier, time_stamp, values)


class AnnotationOSCTranslator(object):

	EVENT_TYPE = Annotation

	def __init__(self):
		self.__routes = {}

	@property
	def routes(self):
		return iter(self.__routes.items())

	def set_route(self, identifier, path):
		self.__routes[identifier] = path

	def get_route(self, annotation):
		variable_type_check(annotation, Annotation)
		return self.__routes.get(annotation.identifier, None)

	def translate(self, annotation):
		variable_type_check(annotation, Annotation)
		path = self.get_route(annotation.identifier)
		if path is not None:
			args = [annotation.time_stamp] + list(self.values)
			return Message(path, args)
		else:
			raise ValueError( 'cannot translate : %s' % annotation )


class AnnotationFileReader(EventFileReader):

	EXPECTED_EXTENSIONS = ['.csv']

	def __init__(self, identifier, file_path):
		if identifier is None:
			identifier = annotation_type_for_annotation_file_name(os.path.split(file_path)[1])
		super(AnnotationFileReader, self).__init__(identifier, file_path)

	@property
	def events(self):
		with open(self.file_path, 'r') as f:
			for line in f:
				annotation = Annotation.new_from_string(self.identifier, line)
				assert annotation
				yield annotation


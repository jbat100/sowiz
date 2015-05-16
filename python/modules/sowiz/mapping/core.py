

from sowiz.util import variable_type_check
from sowiz.description.annotation import Annotation


# Mapper

class EventMapper(object):

	def __init__(self, target_descriptor, target_attribute, mapping):
		super(EventMapper, self).__init__()
		self.__target_descriptor = self.__target_attribute = self.__mapping = None
		self.target_descriptor = target_descriptor
		self.target_attribute = target_attribute
		self.mapping = mapping

	@property
	def target_descriptor(self):
		return self.__target_descriptor

	@target_descriptor.setter
	def target_descriptor(self, target_descriptor):
		self.__target_descriptor = target_descriptor

	@property
	def target_attribute(self):
		return self.__target_attribute

	@target_attribute.setter
	def target_attribute(self, target_attribute):
		self.__target_attribute = target_attribute

	@property
	def mapping(self):
		return self.__mapping

	@mapping.setter
	def mapping(self, mapping):
		self.__mapping = mapping

	def process(self, event):
		raise NotImplementedError()


# Mappings

class EventMapping(object):

	""" Convert an event to a value """

	EVENT_TYPE = None

	def __init__(self):
		self.__processor = None

	@property
	def processor(self):
		return self.__processor

	def check_event(self, event):
		variable_type_check(event, self.EVENT_TYPE)

	def map(self, event):
		pass

	def perform_mapping(self, event):
		raise NotImplementedError()

# Value Annotations

class AnnotationValueMapping(EventMapping):

	EVENT_TYPE = Annotation

	def check_event(self, event):
		super(AnnotationValueMapping, self).check_event(event)
		values = list(event.values)
		if len(values) != 1:
			raise RuntimeError('unexpected number of values')

	def perform_mapping(self, event):
		return list(event.values)[0]

# Grid Annotations

class AnnotationGridMapping(EventMapping):

	EVENT_TYPE = Annotation

	def __init__(self, indeces=None):
		self.__indeces = None
		self.indeces = indeces

	@property
	def indeces(self):
		return iter(self.__indeces)

	@indeces.setter
	def indeces(self, indeces):
		variable_type_check(indeces, list)
		self.__indeces = indeces

	def event_values(self, event):
		if self.indeces:
			return event.get_values_for_indeces(self.indeces)
		else:
			return list(event.values)

	def check_event(self, event):
		super(AnnotationValueMapping, self).check_event(event)
		if event.value_count < 1:
			raise RuntimeError('unexpected number of values')

class AnnotationGridMaxIndexMapping(AnnotationGridMapping):

	def perform_mapping(self, event):
		values = self.event_values(event)
		return values.index(max(values))

class AnnotationGridMaxValueMapping(AnnotationGridMapping):

	def perform_mapping(self, event):
		return max(self.event_values(event))

class AnnotationGridMinIndexMapping(AnnotationGridMapping):

	def perform_mapping(self, event):
		values = self.event_values(event)
		return values.index(min(values))

class AnnotationGridMinValueMapping(AnnotationGridMapping):

	def perform_mapping(self, event):
		return min(self.event_values(event))

class AnnotationGridAverageValueMapping(AnnotationGridMapping):

	def perform_mapping(self, event):
		values = self.event_values(event)
		return sum(values) / len(values)
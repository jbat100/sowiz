
from collections import OrderedDict

from sowiz.util import Event, Identifiable, variable_type_check
from sowiz.network.osc import Message

class SceneDescription(Identifiable):

	def __init__(self, **kwargs):
		super(SceneDescription, self).__init__(**kwargs)


class SceneLayerDescription(Identifiable):

	def __init__(self, **kwargs):
		super(SceneLayerDescription, self).__init__(**kwargs)
		self.__scene = kwargs.pop('scene')
		variable_type_check(self.__scene, SceneDescription)
		self.__descriptors = []

	@property
	def scene(self):
		return self.__scene

	def add_descriptor(self, descriptor):
		variable_type_check(descriptor, SceneDescriptor)
		if descriptor in self.__descriptors:
			raise ValueError('already contains %r' % descriptor)
		self.__descriptors.append(descriptor)

	def remove_descriptor(self, descriptor):
		variable_type_check(descriptor, SceneDescriptor)
		if descriptor not in self.__descriptors:
			raise ValueError('does not contain %r' % descriptor)
		self.__descriptors.remove(descriptor)


class SceneDescriptorOSCTranslator(object):

	PREFIX = '/sowiz/scene/descriptor'

	def translate(self, scene_descriptor):
		path = self.PREFIX + '/' + scene_descriptor.TYPE
		args = list(scene_descriptor.attributes)
		return Message(path, args)


class SceneDescriptor(Identifiable):

	TYPE = None
	ATTRIBUTES = OrderedDict()

	def __init__(self, **kwargs):
		super(SceneDescriptor, self).__init__(**kwargs)
		self.__update_event = Event()
		self.__attributes = {}
		self.__layer = kwargs.pop('layer')
		variable_type_check(self.__layer, SceneLayerDescription)
		attribute_dict = kwargs.pop('attributes')
		self.set_attributes(attribute_dict)

	@property
	def layer(self):
		return self.__layer

	@property
	def update_event(self):
		return self.__update_event

	@property
	def attributes(self):
		for key in self.ATTRIBUTES:
			if key in self.__attributes:
				yield self.__attributes[key]
			else:
				yield self.ATTRIBUTES[key]

	def __getattr__(self, key):
		if key not in self.ATTRIBUTES:
			raise ValueError('invalid attribute key : %s' % key)
		if key in self.__attributes:
			return self.__attributes[key]
		else:
			return self.ATTRIBUTES[key]

	def __setattr__(self, key, value):
		if key not in self.ATTRIBUTES:
			raise ValueError('invalid attribute key : %s' % key)
		self.__attributes[key] = value
		self.__update_event.fire(self, [key])

	def set_attributes(self, attribute_dict):
		for key, new_value in attribute_dict.items():
			if key not in self.ATTRIBUTES:
				raise ValueError('invalid attribute key : %s' % key)
			self.__attributes[key] = new_value
		self.__update_event.fire(self, attribute_dict.keys())


class ActionDescriptor(SceneDescriptor):
	TYPE = 'action'
	ATTRIBUTES = OrderedDict([ ('action', 'default') ])

class ControlDescriptor(SceneDescriptor):
	TYPE = 'control'
	ATTRIBUTES = OrderedDict([ ('channel', 0), ('amplitude', 0) ])

class ImpulseDescriptor(SceneDescriptor):
	TYPE = 'impulse'
	ATTRIBUTES = OrderedDict([ ('channel', 0), ('index', 0), ('amplitude', 0) ])

class RGBAColourDescriptor(SceneDescriptor):
	TYPE = 'rgba'
	ATTRIBUTES = OrderedDict([ ('red', 0), ('green', 0), ('blue', 0), ('alpha', 0) ])

class TextureCoordinateDescriptor(SceneDescriptor):
	TYPE = 'texture_coordinates'
	ATTRIBUTES = OrderedDict([ ('x', 0), ('y', 0), ('z', 0)] )

class ExcitationDescriptor(SceneDescriptor):
	TYPE = 'excitation'
	ATTRIBUTES = OrderedDict([ ('amplitude', 0), ('period', 1) ])

class SwirlDescriptor(SceneDescriptor):
	TYPE = 'swirl'
	ATTRIBUTES = OrderedDict([ ('amplitude', 0), ('period', 1) ])


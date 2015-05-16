

class MappingOutput(object):

	def __init__(self, channel):
		self.__channel = channel

	@property
	def channel(self):
		return self.__channel

	@channel.setter
	def channel(self, channel):
		self.__channel = channel


class DescriptorConnection(object):

	def __init__(self, descriptor, output):
		self.__descriptor = descriptor
		self.__output = output
		self.__descriptor.update_event.handle(self.handle_descriptor_update)

	def handle_descriptor_update(self):
		pass

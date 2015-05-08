class Event(object):

	def __init__(self, identifier, time_stamp):
		self.__identifier = identifier
		self.__time_stamp = time_stamp

	def __str__(self):
		s = self.__class__.__name__ + ' ' + self.identifier + ' time_stamp: ' + str(self.time_stamp)
		return s

	@property
	def identifier(self):
		return self.__identifier

	@property
	def time_stamp(self):
		return self.__time_stamp


class EventReader(object):

	def __init__(self, identifier):
		self.__identifier = identifier

	def __str__(self):
		return self.__class__.__name__ + ' identifier : ' + self.identifier

	@property
	def identifier(self):
		# return the name of the file without extention
		return self.__identifier

	@property
	def events(self):
		return iter([])


class EventFileReader(EventReader):

	EXPECTED_EXTENSIONS = []

	def __init__(self, identifier, file_path):
		super(EventFileReader, self).__init__(identifier)
		self.__file_path = file_path

	def __str__(self):
		return super(EventFileReader, self).__str__() + ' file_path : ' + self.file_path

	@property
	def file_path(self):
		return self.__file_path


class EventOSCTranslator(object):

	EVENT_TYPE = None

	def translate(self, event):
		raise NotImplementedError()

def get_all_event_file_reader_classes():
	return [cls for cls in globals()['EventFileReader'].__subclasses__()]


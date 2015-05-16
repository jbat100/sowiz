
import threading
import logging
import sys

def perform_logging_setup(level = logging.DEBUG, show_time=True, show_level=True):

	""" This sets up the logging module to output to the main output console, should really add some configuration options """

	if perform_logging_setup.counter > 0:
		raise RuntimeError('should not call perform_setup more than once')
	perform_logging_setup.counter += 1
	root = logging.getLogger()
	ch = logging.StreamHandler(sys.stdout)
	ch.setLevel(level)
	formatter_string = ''
	if show_time:
		formatter_string += '%(asctime)s - '
	if show_level:
		formatter_string += '%(levelname)s - '
	formatter_string += '%(message)s'
	formatter = logging.Formatter(formatter_string)
	ch.setFormatter(formatter)
	root.addHandler(ch)
	root.setLevel(level)
	logging.info('logging setup has been performed')

perform_logging_setup.counter = 0


def variable_type_check(variable, expected_type, accept_none = False):

	""" I know this is not a very pythonic way of doing things, duck typing is better in general, however I think
	in early, undocumented stages of development, it is better to enforce expected types instead of silently ignoring incorrect
	use of code """

	if accept_none and variable is None:
		return
	try:
		if not isinstance(variable, expected_type):
			message = 'variable_type_check for %r failed, expected %r'%(variable, expected_type)
			logging.error(message)
			raise ValueError(message)
	except TypeError, e:
		logging.error('expected_type is probably invalid: %r', expected_type)
		raise

#=======================================================================================================================
# Enum
#=======================================================================================================================

class Enum(set):

	""" I really want to do enums and python doesn't really have a good way of doing them up to python 3.4, this is the best way
	I have found (see one of the best answer on http://stackoverflow.com/questions/36932/how-can-i-represent-an-enum-in-python)
	You can use it like this:

	Animals = Enum(["DOG", "CAT", "HORSE"])
	print(Animals.DOG)

	"""

	def __getattr__(self, name):
		if name in self:
			return name
		raise AttributeError('unrecognized enumeration element: %r (known: %s)'%(name, str(self)))

#=======================================================================================================================
# Event
#=======================================================================================================================

class Event(object):

	""" Generic event class taken from http://stackoverflow.com/questions/1092531/event-system-in-python, anyone can subsribe
	or unsubscribe by calling handle or unhandle and the function/method they want to be called, all arguments will be forwarded """

	def __init__(self):
		self.__handlers = set()
		self.__safe = True
		self.__lock = threading.RLock()

	def handle(self, handler):
		with self.__lock:
			self.__handlers.add(handler)
		return self

	def unhandle(self, handler):
		try:
			with self.__lock:
				self.__handlers.remove(handler)
		except ValueError, e:
			logging.debug('%r unhandle called for non-existing handler %r'%(self, handler))
			#raise ValueError("Handler is not handling this event, so cannot unhandle it.")
		return self

	def unhandle_all(self):
		with self.__lock:
			self.__handlers.clear()

	def fire(self, *args, **kargs):
		with self.__lock:
			for handler in self.__handlers:
				try:
					handler(*args, **kargs)
				except Exception, e:
					logging.exception('exception while calling event handler : %r' % handler)
					if not self.__safe:
						raise

	@property
	def handler_count(self):
		with self.__lock:
			return len(self.__handlers)

	__iadd__ = handle
	__isub__ = unhandle
	__call__ = fire
	__len__  = handler_count

#=======================================================================================================================
# Useful threads
#=======================================================================================================================

class StoppableThread(threading.Thread):

	def __init__(self):
		super(StoppableThread, self).__init__()
		self.daemon = True
		self.__stop_event = threading.Event()

	def stop(self):
		self.__stop_event.set()

	def is_stopped(self):
		self.__stop_event.is_set()

	def sleep(self, timeout):
		return self.__stop_event.wait(timeout)

#=======================================================================================================================
# Traits
#=======================================================================================================================

class Identifiable(object):

	def __init__(self, **kwargs):
		self.__identifier = kwargs.pop('identifier', None)



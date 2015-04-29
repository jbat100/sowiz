
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
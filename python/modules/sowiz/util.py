
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
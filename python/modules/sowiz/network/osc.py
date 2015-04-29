
import logging
import datetime
import time
import liblo

from sowiz.util import variable_type_check

class Message(object):

	""" Encapsulated an OSC message as a path and a list of arguments, this is meant to be immutable """

	def __init__(self, path, args = []):
		"""
		:params path: a :py:class:`str` containing the OSC message path/URL
		:params args: a :py:class:`list` containing the arguments for the OSC message
		:raises: :py:exc:`ValueError` if path is not a :py:class:`str` or if args is not a :py:class:`list`
		"""
		path = str(path)
		variable_type_check(path, str)
		variable_type_check(args, list)
		self.__path = path
		self.__args = args
		self.date = None

	def __str__(self):
		return 'OSCMessage(path: %s, args: %s)'%(self.__path, str(self.__args))

	def get_path(self):
		""" :returns: a :py:class:`str` containing the OSC message path/URL """
		return self.__path

	def get_args(self):
		""" :returns: a :py:class:`list` containing the arguments for the OSC message """
		return self.__args


class Client(liblo.Address):

	def __init__(self, hostname, port, namespaces = []):
		self.namespaces = namespaces # str OSCProtocol.PYMOL
		try:
			super(Client, self).__init__(hostname, port)
			logging.info('initialized OSC client with hostname %r port %r'%(hostname, port))
		except liblo.AddressError, err:
			logging.error('could not start OSC client %r'%err)
			raise

	def __str__(self):
		return self.__class__.__name__ + ' (hostname: %s, port: %s)'%(self.get_hostname(), self.get_port())

	def send_message(self, message):
		assert isinstance(message, Message)
		message.date = time.time()
		try:
			liblo.send(self, message.get_path(), *message.get_args())
			logging.debug('%s sent %s'%(self,message))
		except (liblo.AddressError, IOError) as e:
			logging.error('%s could not send %s, error %r'%(self,message,e))
			pass

	def send_messages_as_bundle(self, messages):
		bundle = liblo.Bundle()
		now = time.time()
		for message in messages:
			variable_type_check(message, Message)
			message.date = now
			bundle.add(message.get_path(), *message.get_args())
		try:
			liblo.send(self, bundle)
		except (liblo.AddressError, IOError) as e:
			logging.error('%s could not send %s, error %r'%(self,message,e))
			pass

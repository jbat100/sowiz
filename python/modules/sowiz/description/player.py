import logging
import Queue

from sowiz.util import StoppableThread
from sowiz.network.osc import Message, Client
from sowiz.description.reader import AnnotationReaderThread


class AnnotationClient(object):

	def send(self, annotation):
		"""
		:param annotation: an description which the client should do something with
		:raise NotImplementedError:
		"""
		raise NotImplementedError()

class AnnotationMultiClient(AnnotationClient):

	def __init__(self):
		self.__clients = []

	def add_client(self, client):
		if client not in self.__clients:
			self.__clients.append(client)
		else:
			raise ValueError('already contains client %s' % str(client))

	def remove_client(self, client):
		if client in self.__clients:
			self.__clients.remove(client)
		else:
			raise ValueError('does not contains client %s' % str(client))

	@property
	def clients(self):
		return iter(self.__clients)

	def send(self, annotation):
		for client in self.clients:
			client.send(annotation)


class AnnotationPrintClient(AnnotationClient):

	def send(self, annotation):
		logging.info( 'client received : %s' % str(annotation) )

class AnnotationOSCClient(AnnotationClient):

	def __init__(self, hostname, port):
		self.__osc_client = Client(hostname, port)
		self.__routes = {}

	@property
	def osc_client(self):
		return self.__osc_client

	@property
	def routes(self):
		return iter(self.__routes.items())

	def get_route(self, identifier):
		"""
		:rtype : str
		:param identifier: an description identifier
		:return: osc path associated with the description identifier
		"""
		return self.__routes.get(identifier, None)

	def set_route(self, identifier, path):
		self.__routes[identifier] = path

	def send(self, annotation):
		path = self.get_route(annotation.identifier)
		if path is not None:
			args = [annotation.time_stamp]
			for value in annotation.values:
				args.append(value)
			self.osc_client.send_message( Message(path, args) )



class AnnotationPlayerThread(StoppableThread):

	def __init__(self, client, queue):
		super(AnnotationPlayerThread, self).__init__()
		self.__queue = queue
		self.__client = client

	@property
	def queue(self):
		return self.__queue

	@property
	def client(self):
		return self.__client

	def run(self):
		logging.debug('starting player thread')
		while True:
			annotation = self.queue.get()
			logging.debug('player thread got %s' % str(annotation))
			if annotation is not None:
				self.client.send(annotation)
			elif self.is_stopped():
				break


class AnnotationPlayer(object):

	def __init__(self, client):
		self.__client = client
		self.__readers = []
		self.__player_thread = None
		self.__reader_threads = []
		self.__queue = Queue.Queue()

	def add_reader(self, reader):
		logging.debug('adding reader %s' % str(reader))
		if reader not in self.__readers:
			self.__readers.append(reader)
		else:
			raise ValueError(u"already contains reader : {0:s}".format(str(reader)))

	@property
	def readers(self):
		return iter(self.__readers)

	@property
	def client(self):
		return self.__client

	@property
	def queue(self):
		return self.__queue

	def play(self):
		logging.info('annotations playing')
		self._stop_internal_threads()
		self.__player_thread = AnnotationPlayerThread(self.client, self.queue)
		self.__player_thread.start()
		for reader in self.readers:
			reader_thread = AnnotationReaderThread(reader, self.queue)
			self.__reader_threads.append(reader_thread)
			reader_thread.start()

	def stop(self):
		self._stop_internal_threads()
		logging.info('annotations stopped')

	def _stop_internal_threads(self):
		while len(self.__reader_threads) > 0:
			self.__reader_threads.pop().stop()
		if self.__player_thread is not None:
			self.__player_thread.stop()
			self.__player_thread.queue.put(None)
			self.__player_thread = None





import logging
import Queue
import time

from sowiz.util import variable_type_check
from sowiz.network.osc import Client
from sowiz.description.core import EventClient, EventOSCTranslator
from sowiz.util import StoppableThread


class EventPrintClient(EventClient):

	def send(self, event):
		logging.info( 'client received : %s' % str(event) )


class EventOSCClient(EventClient):

	def __init__(self, hostname, port):
		self.__osc_client = Client(hostname, port)
		self.__translators = []

	@property
	def osc_client(self):
		return self.__osc_client

	@property
	def translators(self):
		return iter(self.__translators)

	def register_translator(self, translator):
		variable_type_check(translator, EventOSCTranslator)
		if translator not in self.__translators:
			self.__translators.append(translator)
		else:
			raise ValueError('already contains %s' % translator)

	def send(self, event):
		for translator in self.translators:
			if isinstance(event, translator.EVENT_TYPE):
				message = translator.translate(event)
				self.osc_client.send_message( message )



class EventPlayerThread(StoppableThread):

	def __init__(self, client, queue):
		super(EventPlayerThread, self).__init__()
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
			reader, event = self.queue.get()
			logging.debug('player thread got %s' % str(event))
			if event is not None:
				self.client.send(reader, event)
			elif self.is_stopped():
				break


class EventReaderThread(StoppableThread):

	def __init__(self, reader, queue):
		super(EventReaderThread, self).__init__()
		self.__reader  = reader
		self.__queue = queue
		self.__start_time = None

	@property
	def reader(self):
		return self.__reader

	@property
	def queue(self):
		return self.__queue

	@property
	def start_time(self):
		return self.__start_time

	def run(self):
		logging.debug('starting reader thread %s' % str(self.reader))
		self.__start_time = time.time()
		for event in self.reader.events:
			wait_time = self.start_time + event.time_stamp - time.time()
			logging.debug('next description in %f seconds' % wait_time)
			if wait_time > 0.0:
				self.sleep(wait_time)
			if self.is_stopped():
				break
			self.queue.put((self.reader, event))
		logging.debug('ending reader thread %s' % str(self.reader))


class EventPlayer(object):

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
		self.__player_thread = EventPlayerThread(self.client, self.queue)
		self.__player_thread.start()
		for reader in self.readers:
			reader_thread = EventReaderThread(reader, self.queue)
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


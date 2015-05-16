
import math

from sowiz.util import variable_type_check

#===============================================================================
# Processors
#===============================================================================

# Processors

class Processor(object):

	def __init__(self):
		self.__next_processor = None

	@property
	def next_processor(self):
		return self.__next_processor

	@next_processor.setter
	def next_processor(self, next_processor):
		variable_type_check(next_processor, Processor)
		self.__next_processor = next_processor

	@property
	def chain(self):
		chain = [self] + self.next_processor.processor_chain if self.next_processor else []
		return iter(chain)

	@property
	def chain_length(self):
		return 1 if self.next_processor is None else 1 + self.next_processor.chain_length

	def add_processor(self, processor):
		self.next_processor = processor
		return processor

	def process(self, value, timestamp=None):
		raise NotImplementedError()

	def __call__(self, value, timestamp=None):
		processed = self.process(value, timestamp)
		return self.next_processor(processed, timestamp) if self.next_processor is not None else processed

	
class PassProcessor(Processor):
	
	def process(self, value, timestamp=None):
		return value

class LinearFunction(Processor):
	
	def __init__(self, scale=1.0, offset=0.0):
		super(LinearFunction, self).__init__()
		self.__scale = float(scale)
		self.__offset = float(offset)

	@property
	def scale(self):
		return self.__scale

	@scale.setter
	def scale(self, scale):
		self.__scale = scale

	@property
	def offset(self):
		return self.__offset

	@offset.setter
	def offset(self, offset):
		self.__offset = offset
		
	def process(self, value, timestamp=None):
		return (value * self.scale) + self.offset

# http://www.lifl.fr/~casiez/1euro/OneEuroFilter.py

class LowPassFilter(Processor):

	def __init__(self, alpha):
		super(LowPassFilter, self).__init__()
		self.__alpha = self.__y = self.__s = None
		self.alpha = alpha

	@property
	def alpha(self):
		return self.__alpha

	@alpha.setter
	def alpha(self, alpha):
		alpha = float(alpha)
		if alpha<=0 or alpha>1.0:
			raise ValueError("alpha (%s) should be in (0.0, 1.0]"%alpha)
		self.__alpha = alpha

	@property
	def last_value(self):
		return self.__y

	def process(self, value, timestamp=None):
		if self.__y is None:
			s = value
		else:
			s = (self.__alpha * value) + ((1.0-self.__alpha) * self.__s)
		self.__y = value
		self.__s = s
		return s

# http://www.lifl.fr/~casiez/1euro/OneEuroFilter.py
# note default values in the WIS for the one euro filter are mincutoff=1.0, beta=1.5, freq is not a
# very useful parameter (only affects the first output)

class OneEuroFilter(Processor):

	def __init__(self, freq, mincutoff=1.0, beta=0.0, dcutoff=1.0):
		super(OneEuroFilter, self).__init__()
		if freq<=0:
			raise ValueError("freq should be >0")
		if mincutoff<=0:
			raise ValueError("mincutoff should be >0")
		if dcutoff<=0:
			raise ValueError("dcutoff should be >0")
		self.__freq = float(freq)
		self.__mincutoff = float(mincutoff)
		self.__beta = float(beta)
		self.__dcutoff = float(dcutoff)
		self.__x = LowPassFilter(self.alpha(self.__mincutoff))
		self.__dx = LowPassFilter(self.alpha(self.__dcutoff))
		self.__lasttime = None
		
	def alpha(self, cutoff):
		te	= 1.0 / self.__freq
		tau   = 1.0 / (2*math.pi*cutoff)
		return  1.0 / (1.0 + tau/te)

	def process(self, x, timestamp=None):
		# ---- update the sampling frequency based on timestamps
		if self.__lasttime and timestamp:
			self.__freq = 1.0 / (timestamp-self.__lasttime)
		self.__lasttime = timestamp
		# ---- estimate the current variation per second
		prev_x = self.__x.last_value()
		dx = 0.0 if prev_x is None else (x-prev_x)*self.__freq # FIXME: 0.0 or value?
		self.__dx.set_alpha(self.alpha(self.__dcutoff))
		edx = self.__dx(dx, timestamp)
		# ---- use it to update the cutoff frequency
		cutoff = self.__mincutoff + self.__beta*math.fabs(edx)
		# ---- filter the given value
		self.__x.set_alpha(self.alpha(cutoff))
		return self.__x(x, timestamp)
	
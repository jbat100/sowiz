
import argparse
import time

import liblo

def main():

	# oscsend localhost 8000 /analyser1/default/magnitude/something f 0.3

	parser = argparse.ArgumentParser(description="Genrates OSC test messages for Sowiz")
	parser.add_argument("-t", "--target", type=str, default='localhost', help="OSC server host")
	parser.add_argument("-p", "--port", type=int, default=3333, help="OSC server port")
	parser.add_argument("-i", "--interval", type=int, default=100, help="Test message time interval (milliseconds)")
	parser.add_argument("-g", "--group", type=str, default='default', help="Sowiz group")
	parser.add_argument("-d", "--descriptor", type=str, default='magnitude', help="Sowiz descriptor")
	parser.add_argument("-l", "--lower-limit", type=float, default=0.0, help="Lower range limit")
	parser.add_argument("-u", "--upper-limit", type=float, default=1.0, help="Upper range limit")
	parser.add_argument("-s", "--step", type=float, default=0.1, help="Upper range limit")
	args = parser.parse_args()

	#client = liblo.Address(args.target, args.port)

	client = liblo.Address(args.target, int(args.port))

	try:
		sleep_interval = float(args.interval) / 1000.0
		current_value = args.lower_limit
		osc_path = '/' + args.group + '/' + args.descriptor
		while True:
			current_value += args.step
			if current_value > args.upper_limit:
				current_value = args.lower_limit
			print 'Sending test message ' + osc_path + ' ' + str(current_value)
			liblo.send(client, osc_path, current_value)
			time.sleep(sleep_interval)
	except (RuntimeError, KeyboardInterrupt, SystemExit, EOFError), e:
		print e

	print 'Ending Sowiz test messages'



if __name__ == "__main__":
	main()

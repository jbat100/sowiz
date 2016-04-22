
import argparse
import time

import liblo

def main():

	# oscsend localhost 8000 /analyser1/default/magnitude/something f 0.3

	print "Hello"

	parser = argparse.ArgumentParser(description="Genrates OSC test midi messages for Sowiz")

	parser.add_argument("-t", "--target", type=str, default='localhost', help="OSC server host")
	parser.add_argument("-p", "--port", type=int, default=3333, help="OSC server port")
	parser.add_argument("-g", "--group", type=str, default='default', help="Sowiz group")

	parser.add_argument("-i", "--interval", type=int, default=1000, help="Note interval (milliseconds)")
	parser.add_argument("-d", "--duration", type=int, default=1000, help="Note duration (milliseconds)")
	parser.add_argument("-x", "--parameter", type=str, default='pitch', help="Midi note parameter (channel, pitch, velocity)")
	parser.add_argument("-l", "--lower-limit", type=int, default=0, help="Lower range limit")
	parser.add_argument("-u", "--upper-limit", type=int, default=127, help="Upper range limit")
	parser.add_argument("-s", "--step", type=int, default=1, help="Upper range limit")
	args = parser.parse_args()

	#client = liblo.Address(args.target, args.port)

	client = liblo.Address(args.target, int(args.port))

	try:
		interval = float(args.interval) / 1000.0
		duration = float(args.duration) / 1000.0
		parameter_index = {"channel":0, "pitch":1, "velocity":2}[args.parameter]
		current_value = args.lower_limit
		osc_path = '/' + args.group + '/midi'
		while True:
			current_value += args.step
			if current_value > args.upper_limit:
				current_value = args.lower_limit
			note_parameters = [64, 64, 64]
			note_parameters[parameter_index] = current_value
			print "Note on " + str(note_parameters)
			liblo.send(client, osc_path, "note_on", *note_parameters)
			time.sleep(duration)
			print "Note off " + str(note_parameters)
			liblo.send(client, osc_path, "note_off", *note_parameters)
			time.sleep(interval)
	except (RuntimeError, KeyboardInterrupt, SystemExit, EOFError), e:
		print e

	print 'Ending Sowiz test messages'



if __name__ == "__main__":
	print "main"
	main()
else:
	print "not main"

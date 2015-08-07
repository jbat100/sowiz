

#import "Extractor_UIView.h"



#pragma mark ____ LISTENER CALLBACK DISPATCHER ____

// This listener responds to parameter changes, gestures, and property notifications
void EventListenerDispatcher (void *inRefCon, void *inObject, const AudioUnitEvent *inEvent, UInt64 inHostTime, Float32 inValue)
{
	
}

@implementation Extractor_UIView

-(void) awakeFromNib {
	NSString *path = [[NSBundle bundleForClass: [Extractor_UIView class]] pathForImageResource: @"SectionPatternLight"];
	NSImage *pattern = [[NSImage alloc] initByReferencingFile: path];
	mBackgroundColor = [[NSColor colorWithPatternImage: [pattern autorelease]] retain];
}

#pragma mark ____ (INIT /) DEALLOC ____
- (void)dealloc {
	[mBackgroundColor release];
	
	[[NSNotificationCenter defaultCenter] removeObserver: self];
	
    [super dealloc];
}

#pragma mark ____ PUBLIC FUNCTIONS ____
- (void)setAU:(AudioUnit)inAU {
	// remove previous listeners
	mAU = inAU;
}

- (void)drawRect:(NSRect)rect
{
	[mBackgroundColor set];
	NSRectFill(rect);		// this call is much faster than using NSBezierPath, but it doesn't handle non-opaque colors
	
	[super drawRect: rect];	// we call super to draw all other controls after we have filled the background
}

#pragma mark ____ INTERFACE ACTIONS ____

/* If we get a mouseDown, that means it was not in the graph view, or one of the text fields. 
   In this case, we should make the window the first responder. This will deselect our text fields if they are active. */
- (void) mouseDown: (NSEvent *) theEvent {
	[super mouseDown: theEvent];
	[[self window] makeFirstResponder: self];
}

- (BOOL) acceptsFirstResponder {
	return YES;
}

- (BOOL) becomeFirstResponder {	
	return YES;
}

- (BOOL) isOpaque {
	return YES;
}

@end

#if GMA_PREVIEW_FEATURES
#import "TestPlugin_Preview.h"

@implementation TestPlugin (Preview)

+ (void)HelloWorldPreview {
    NSLog(@"Hello World from TestPlugin_Preview");
}

@end
#endif

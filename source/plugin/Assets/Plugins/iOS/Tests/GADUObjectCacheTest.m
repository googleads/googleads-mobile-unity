#import <XCTest/XCTest.h>

#import "GADUObjectCache.h"

@interface GADUObjectCacheTest : XCTestCase
@end

@implementation GADUObjectCacheTest

- (void)setUp {
  [super setUp];
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache removeObjectForKey:@"test_key"];
  [cache removeObjectForKey:@"test_key_subscript"];
}

- (void)tearDown {
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  [cache removeObjectForKey:@"test_key"];
  [cache removeObjectForKey:@"test_key_subscript"];
  [super tearDown];
}

- (void)testSharedInstanceReturnsSingletonInstance {
  GADUObjectCache *cache1 = [GADUObjectCache sharedInstance];
  GADUObjectCache *cache2 = [GADUObjectCache sharedInstance];

  XCTAssertNotNil(cache1);
  XCTAssertEqual(cache1, cache2);
}

- (void)testReferencesPropertyReturnsSelf {
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];

  GADUObjectCache *references = cache.references;

  XCTAssertEqual(cache, references);
}

- (void)testSetObjectAndGetObjectForKey {
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  NSString *objectToStore = @"cached_value";
  NSString *key = @"test_key";

  [cache setObject:objectToStore forKey:key];
  id retrievedObject = [cache objectForKey:key];

  XCTAssertEqualObjects(retrievedObject, objectToStore);
}

- (void)testSubscriptAccessSetAndGet {
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  NSString *objectToStore = @"subscript_value";
  NSString *key = @"test_key_subscript";

  cache[key] = objectToStore;
  id retrievedObject = cache[key];

  XCTAssertEqualObjects(retrievedObject, objectToStore);
}

- (void)testRemoveObjectForKeyRemovesObjectFromCache {
  GADUObjectCache *cache = [GADUObjectCache sharedInstance];
  NSString *objectToStore = @"temporary_value";
  NSString *key = @"test_key";

  [cache setObject:objectToStore forKey:key];
  [cache removeObjectForKey:key];
  id retrievedObject = [cache objectForKey:key];

  XCTAssertNil(retrievedObject);
}

- (void)testObjectReferenceKeyGeneratesNonEmptyKey {
  NSObject *testObject = [[NSObject alloc] init];

  NSString *referenceKey = [testObject gadu_referenceKey];

  XCTAssertNotNil(referenceKey);
  XCTAssertGreaterThan(referenceKey.length, 0);
}

@end

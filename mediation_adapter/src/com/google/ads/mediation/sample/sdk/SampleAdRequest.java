package com.google.ads.mediation.sample.sdk;

import java.util.Set;

/**
 * A sample ad request used to load an ad. This is an example of some targeting options an ad
 * network may provide.
 */
public class SampleAdRequest {

  /**
   * Creates a new {@link SampleAdRequest}.
   */
  public SampleAdRequest() {
  }

  /**
   * Sets keywords for targeting purposes.
   * @param keywords A set of keywords.
   */
  public void setKeywords(Set<String> keywords) {
    // Normally we'd save the keywords. But since this is a sample network, we'll do nothing.
  }

  /**
   * Designates a request for test mode.
   * @param useTesting {@code true} to enable test mode.
   */
  public void setTestMode(boolean useTesting) {
    // Normally we'd save this flag. But since this is a sample network, we'll do nothing.
  }
}

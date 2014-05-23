package com.google.ads.mediation.sample.sdk;

/**
 * The size of an ad request to the sample ad network.
 */
public class SampleAdSize {
  private int width;
  private int height;

  public SampleAdSize(int width, int height) {
    this.width = width;
    this.height = height;
  }

  public int getWidth() {
    return width;
  }

  public int getHeight() {
    return height;
  }
}

// Copyright (C) 2020 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace GoogleMobileAds.Api
{
   public class MaxAdContentRating {
      private MaxAdContentRating(string value){Value = value;}
      public string Value {get; set;}
      public static MaxAdContentRating G {get{return new MaxAdContentRating("G");}}
      public static MaxAdContentRating MA {get{return new MaxAdContentRating("MA");}}
      public static MaxAdContentRating PG {get{return new MaxAdContentRating("PG");}}
      public static MaxAdContentRating T {get{return new MaxAdContentRating("T");}}
      public static MaxAdContentRating Unspecified {get{return new MaxAdContentRating("");}}

      public static MaxAdContentRating ToMaxAdContentRating(string value){
         return new MaxAdContentRating(value);
      }
   }
}

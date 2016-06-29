// Copyright CommonsForNET.
// Licensed to the Apache Software Foundation (ASF) under one or more
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership.
// The ASF licenses this file to You under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyCopyright("")]
[assembly: ComVisible(false)]

[assembly: Guid("84e7ec63-7b50-4dc1-ae96-c3b56626efd7")]

// * For versioning, the fourth digit represents the days from the build date to the 
//   day of the first release (2/15/2015).
// * The third digit is increased when a small feature is added.
// * The second digit is increased when a component is added to the whole package.
// * The first digit is increase when a set of components planned yearly are completed.
// * The nuget package version shall be the same with the assembly version.
// * Each new release brought up by the nuget package forces re-compile for 
//   the applications and components which depends on the .NET Commons library.
// * The build number only changes when a new nuget package is uploaded. If there are more than 
//   one upload in one day, the build number is increased by one. If next few days, another upload happens,
//   build number is increased by one if the number of days to the first day is less than or 
//   equal to the build number.
// * TODO: Automated version strategy will be developed in future.
[assembly: AssemblyFileVersion("0.2.3")]
[assembly: CLSCompliant(true)]

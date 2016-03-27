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

namespace Commons.Json
{
    internal static class Messages
    {
        public const string InvalidFormat = "The format of the JSON string is invalid.";

        public const string InvalidValue = "The json value is invalid";

        public const string JsonObjectCannotIndex = "The indexer cannot be applied to a json object";

	    public const string OutOfRange = "The index is out of the JSON array range.";

	    public const string IndexerCannotApply = "The indexer type cannot be applied to the JSON array.";

	    public const string CannotConvertToArray = "Cannot convert the JSON array to the specified object.";

        public const string NoDefaultConstructor = "The type does not contain a default constructor so it cannot be instantiated.";

        public const string JsonValueTypeNotMatch = "The type of the JSON value does not match the type of the object property.";

        public const string AddMethod = "Add";

        public const string TypeNotSupported = "The type mapped is not supported by the JsonMapper.";

        public const string InvalidProperty = "The expression does not contain a valid property.";

        public const string FieldNotProperty = "The expression only contains a field not a property.";

	    public const string NoPropertyToMap = "No property is not defined yet.";

	    public const string InvalidDateFormat = "The date time format is invalid.";

        public const string NotEnum = "The type is not an Enum.";
    }
}

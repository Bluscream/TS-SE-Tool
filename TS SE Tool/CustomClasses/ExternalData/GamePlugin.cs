/*
   Copyright 2016-2020 LIPtoH <liptoh.codebase@gmail.com>

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System.IO;
using System.Collections.Generic;
using TS_SE_Tool.Utilities;
using System;

namespace TS_SE_Tool {
    public class GamePlugin {
        public bool Enabled {
            get => File32bit.IsDisabled() || File64bit.IsDisabled();
            set {
                File32bit.Enable();
            }
        }
        public FileInfo File32bit { get; private set; }
        public FileInfo File64bit { get; private set; }
        //public DateTime LastModifiedDate { get; private set; }

        public GamePlugin(string fileName) {

        }

        public bool Toggle(bool? enable = false) {

        }
    }
}

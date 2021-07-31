﻿//////////////////////////////////////////////////////////////////////
// Algorithmia is (c) 2018 Solutions Design. All rights reserved.
// https://github.com/SolutionsDesign/Algorithmia
//////////////////////////////////////////////////////////////////////
// COPYRIGHTS:
// Copyright (c) 2018 Solutions Design. All rights reserved.
// 
// The Algorithmia library sourcecode and its accompanying tools, tests and support code
// are released under the following license: (BSD2)
// ----------------------------------------------------------------------
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met: 
//
// 1) Redistributions of source code must retain the above copyright notice, this list of 
//    conditions and the following disclaimer. 
// 2) Redistributions in binary form must reproduce the above copyright notice, this list of 
//    conditions and the following disclaimer in the documentation and/or other materials 
//    provided with the distribution. 
// 
// THIS SOFTWARE IS PROVIDED BY SOLUTIONS DESIGN ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL SOLUTIONS DESIGN OR CONTRIBUTORS BE LIABLE FOR 
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
// BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
//
// The views and conclusions contained in the software and documentation are those of the authors 
// and should not be interpreted as representing official policies, either expressed or implied, 
// of Solutions Design. 
//
//////////////////////////////////////////////////////////////////////
// Contributers to the code:
//		- Frans Bouma [FB]
//////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SD.Tools.Algorithmia.UtilityClasses
{
	/// <summary>
	/// Class which contains argument verification logic and which can throw exceptions if necessary. This code makes it easier to verify input arguments. 
	/// </summary>
	public static class ArgumentVerifier
	{
		/// <summary>
		/// Checks the argument passed in. if it's null, it throws an ArgumentNullException
		/// </summary>
		/// <param name="argument">The argument.</param>
		/// <param name="name">The name.</param>
		public static void CantBeNull(object argument, string name)
		{
			if(argument == null)
			{
				throw new ArgumentNullException(name);
			}
		}


		/// <summary>
		/// Checks if the argument returns true with the func passed in. If not, it throws an ArgumentException with the error message specified. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="checkFunc">The check func.</param>
		/// <param name="argument">The argument.</param>
		/// <param name="formattedError">The formatted error message.</param>
		public static void ShouldBeTrue<T>(Func<T, bool> checkFunc, T argument, string formattedError)
		{
			CantBeNull(checkFunc, "checkFunc");
			if(!checkFunc(argument))
			{
				throw new ArgumentException(formattedError);
			}
		}
	}
}

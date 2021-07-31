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

namespace SD.Tools.Algorithmia.GeneralDataStructures.EventArguments
{
	/// <summary>
	/// Event args class which is used in events which are cancelable and which arguments have to contain the element involved to allow observers
	/// to examine the element involved to better decide what to do: cancel or allow.
	/// </summary>
	/// <typeparam name="T">type of the elements contained in the raising container class</typeparam>
	[Serializable]
	public class CancelableListModificationEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CancelableListModificationEventArgs&lt;T&gt;"/> class.
		/// </summary>
		public CancelableListModificationEventArgs()
			: this(default(T))
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="CancelableListModificationEventArgs&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="involvedElement">The involved element.</param>
		public CancelableListModificationEventArgs(T involvedElement)
			: base()
		{
			this.InvolvedElement = involvedElement;
		}


		#region Class Property Declarations
		/// <summary>
		/// Gets or sets a value indicating whether the event raised and passing this event arguments instance is cancelled or not.
		/// </summary>
		public bool Cancel { get; set; }
		/// <summary>
		/// Gets or sets the involved element.
		/// </summary>
		public T InvolvedElement { get; set; }
		#endregion
	}
}

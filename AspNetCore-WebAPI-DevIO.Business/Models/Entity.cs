using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore_WebAPI_DevIO.Business.Models
{
	public class Entity
	{
		protected Entity()
		{
			Id = Guid.NewGuid();
		}

		public Guid Id { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo
{
	public class TreeDto
	{
		private List<TreeDto> _nodes = new List<TreeDto>();

		public String Id { get; private set; }
		public String Text { get; private set; }
		public IEnumerable<TreeDto> Nodes
		{
			get
			{
				return this._nodes.Count == 0
					? null
					: this._nodes;
			}
			private set { }
		}

		public TreeDto(String id, String text, Boolean isLazy = false)
		{
			this.Id = id;
			this.Text = text;
		}

		public TreeDto AddNode(TreeDto node)
		{
			this._nodes.Add(node);
			return node;
		}

		public TreeDto GetNode(String text)
		{
			foreach(TreeDto node in this._nodes)
				if(node.Text == text)
					return node;

			return null;
		}
	}
}
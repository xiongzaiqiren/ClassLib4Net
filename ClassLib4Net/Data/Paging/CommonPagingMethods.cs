using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib4Net.Data.Paging
{
	/// <summary>
	/// 分页相关的公用方法
	/// </summary>
	public class CommonPagingMethods
	{
		#region 计算分页页数的方法

		/// <summary>
		///  取得分页页数数
		/// </summary>
		/// <param name="dataCount">数据总记录数</param>
		/// <param name="PageSize">每页显示记录数</param>
		/// <returns>页面总数</returns>
		public static int ComputePageCount(long dataCount, int PageSize)
		{
			int count = (int)dataCount / PageSize;
			if (dataCount % PageSize > 0)
			{
				count++;
			}
			return count;
		}

		#endregion
	}
}

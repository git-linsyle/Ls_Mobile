/****************************************************
	�ļ���IActionExecutor.cs
	Author��JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date��2021/01/15 9:14   	
	Features��
*****************************************************/

namespace EdgeFramework
{
    public interface IActionExecutor 
    {
        void ExecuteAction(IAction action);
    }

    public class MonoExecutor : IActionExecutor
    {
        public void ExecuteAction(IAction action)
        {
            
        }
    }
}
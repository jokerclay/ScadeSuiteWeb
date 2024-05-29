namespace ScadeSuiteWeb.Server.Models;


public class Book
{
    public int Id { get; set; }

    /// <summary>
    /// 书名
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 作者
    /// </summary>
    public string Author { get; set; } = string.Empty;
    
    
    /// <summary>
    /// 发行年份
    /// </summary>
    public DateTime PublishYear { get; set; } 
    
    
    /// <summary>
    /// 已删除
    /// </summary>
    public bool IsDeleted { get; set; } 
}

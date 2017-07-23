using System.Collections.Generic;

public interface IHighscoreDataService
{
    void Deinitialize();
    IEnumerable<PlayerDTO> GetHighscoreList();
    void Initialize();
}
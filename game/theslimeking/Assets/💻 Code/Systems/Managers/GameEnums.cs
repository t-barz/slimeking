namespace SlimeKing.Core
{
    /// <summary>
    /// Estados principais do jogo
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Loading,
        Settings
    }
    
    /// <summary>
    /// Estágios de evolução do Slime
    /// </summary>
    public enum SlimeStage
    {
        Baby,       // Estado inicial - movimentação básica
        Adult,      // 4 habilidades elementais desbloqueadas
        Large,      // Combinações elementais avançadas
        King        // Maestria completa dos elementos
    }
    
    /// <summary>
    /// Estações do ciclo temporal
    /// </summary>
    public enum Season
    {
        Spring,     // Renovação, flores emergem
        Summer,     // Vegetação máxima, dias longos
        Autumn,     // Folhagem dourada, névoas
        Winter      // Neve, noites longas
    }
    
    /// <summary>
    /// Tipos de clima dinâmico
    /// </summary>
    public enum WeatherType
    {
        Clear,      // Sol claro - visibilidade máxima
        Cloudy,     // Nublado - iluminação difusa
        Rain,       // Chuva - vegetação cresce, cristais de água recarregam
        Storm,      // Tempestade - raios carregam cristais elétricos
        Snow,       // Neve - rastros visíveis, cristais de gelo
        Fog,        // Névoa - visibilidade limitada, criaturas fantasma ativas
        Aurora      // Aurora Cristalina - evento especial, todos cristais brilham
    }
    
    /// <summary>
    /// Tipos de elementos dos cristais
    /// </summary>
    public enum ElementType
    {
        Fire,       // Vermelho - Câmaras de Lava
        Water,      // Azul - Lago Espelhado
        Earth,      // Marrom - Área Rochosa
        Air,        // Branco - Pico Nevado
        Nature,     // Verde - Floresta Calma
        Shadow      // Roxo - Pântano das Névoas
    }
    
    /// <summary>
    /// Tipos de biomas das Montanhas Cristalinas
    /// </summary>
    public enum BiomeType
    {
        Nest,       // Ninho do Slime - Tutorial
        Forest,     // Floresta Calma - Nature/Earth/Air
        Lake,       // Lago Espelhado - Water/Air
        Rock,       // Área Rochosa - Earth/Fire
        Swamp,      // Pântano das Névoas - Shadow/Water/Nature
        Volcano,    // Câmaras de Lava - Fire/Earth
        Snow        // Pico Nevado - Air/Water
    }
    
    /// <summary>
    /// Períodos do ciclo dia/noite
    /// </summary>
    public enum TimeOfDay
    {
        Dawn,       // 05:00-06:59 - Transição suave
        Morning,    // 07:00-11:59 - Criaturas diurnas ativas
        Afternoon,  // 12:00-17:59 - Pico de atividade
        Evening,    // 18:00-20:59 - Criaturas crepusculares
        Night       // 21:00-04:59 - Criaturas noturnas exclusivas
    }
}
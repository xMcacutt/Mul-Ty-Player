using MulTyPlayerClient.GUI;

namespace MulTyPlayerClient.Classes.GamePlay;

public class HardcoreModeHandler
{
    public void Initialize()
    {
        
    }

    public void InitializeLevel(int level)
    {
        
    }

    public void Deinitialize()
    {
        
    }

    public void Run()
    {
        // Set Hardcore Mode
        ProcessHandler.WriteData(SyncHandler.SaveDataBaseAddress - 0x2, new byte[] { 0x1 });
        // Read Health
        var health = Client.HHero.GetHealth();
        // Set Health
        if (health > 1)
            Client.HHero.SetHealth(1);

        var isBull = Client.HHero.IsBull();
        var state = Client.HHero.GetHeroState();
        if ((isBull && (BullState)state == BullState.Dying) || !isBull && (HeroState)state == HeroState.Dying)
        {
            // Send Death Message
        }
        
        // Check TEs Hub 1
        
        // Check TEs Hub 2
        
        // Check TEs Hub 3
        
        Client.HHero.SetRunSpeed(12.5f);
        Client.HHero.SetAirSpeed(12.5f);
        Client.HHero.SetSwimSpeed(25f);
        Client.HHero.SetJumpHeight(20f);
    }
}
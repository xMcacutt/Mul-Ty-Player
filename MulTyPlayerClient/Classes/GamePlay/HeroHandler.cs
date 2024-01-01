using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

public class HeroHandler
{
    private const int TY_POSITION_ADDRESS = 0x270B78;
    private const int TY_ROTATION_ADDRESS = 0x271C1C;

    private const int BP_POSITION_ADDRESS = 0x254268;
    private const int BP_ROTATION_ADDRESS = 0x2545F0;

    private readonly float[] currentPositionRotation;

    private int positionAddress = TY_POSITION_ADDRESS;
    private int rotationAddress = TY_ROTATION_ADDRESS;

    public HeroHandler()
    {
        currentPositionRotation = new float[6];
        HLevel.OnLevelChange += CheckOutbackSafari;
    }

    private static LevelHandler HLevel => Client.HLevel;
    private static GameStateHandler HGameState => Client.HGameState;

    public void GetTyPosRot()
    {
        for (var i = 0; i < 3; i++)
        {
            ProcessHandler.TryRead(positionAddress + sizeof(float) * i, out currentPositionRotation[i], true,
                "HeroHandler::GetTyPosRot() {position}");
            ProcessHandler.TryRead(rotationAddress + sizeof(float) * i, out currentPositionRotation[i + 3], true,
                "HeroHandler::GetTyPosRot() {rotation}");
        }
    }

    public void CheckOutbackSafari(int levelId)
    {
        if (levelId == Levels.OutbackSafari.Id)
        {
            positionAddress = BP_POSITION_ADDRESS;
            rotationAddress = BP_ROTATION_ADDRESS;
        }
        else
        {
            positionAddress = TY_POSITION_ADDRESS;
            rotationAddress = TY_ROTATION_ADDRESS;
        }
    }

    public void SendCoordinates()
    {
        //Debug.WriteLine("SENDING KOALA COORDS");
        //SENDS CURRENT COORDINATES TO SERVER WITH CURRENT LEVEL AND LOADING STATE
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.PlayerInfo);
        message.AddBool(HGameState.IsAtMainMenu());
        message.AddInt(HLevel.CurrentLevelId);
        message.AddFloats(Client.HHero.currentPositionRotation);
        Client._client.Send(message);
    }
}
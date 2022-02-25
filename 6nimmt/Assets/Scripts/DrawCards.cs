using Mirror;

public class DrawCards : NetworkBehaviour
{
    private PlayerManager _playerManager;

    public void OnClick() 
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        _playerManager = networkIdentity.GetComponent<PlayerManager>();  
        _playerManager.CmdDealCards();
    }
}

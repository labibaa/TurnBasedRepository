using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using Unity.Services.Core;


using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class RelayNetwork : MonoBehaviour
{


    [SerializeField]
    TMP_InputField joinCodeInput;
    [SerializeField]
    TMP_Text roomJoinCode;
    [SerializeField]
    GameObject codeInputField;
    [SerializeField]
    GameObject serverBtn;
    [SerializeField]
    GameObject clientBtn;
    [SerializeField]
    GameObject hostBtn;

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();


        //AuthenticationService.Instance.SignedIn += (() => {
        //    Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        //});
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    //public async void CreateRelay()
    //{
        //try
        //{
            //Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            //string joinCodeHost = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        //    Debug.Log("JoinCode:" + joinCodeHost);
        //    NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
        //        allocation.RelayServer.IpV4,
        //        (ushort)allocation.RelayServer.Port,
        //        allocation.AllocationIdBytes,
        //        allocation.Key,
        //        allocation.ConnectionData

        //        );
        //    roomJoinCode.text = joinCodeHost;
        //    NetworkManager.Singleton.StartHost();
        //    codeInputField.SetActive(false);
        //    serverBtn.SetActive(false);
        //    clientBtn.SetActive(false);
        //    hostBtn.SetActive(false);

        //}
        //catch (RelayServiceException exception)
        //{
        //    Debug.Log(exception.Message);
        //}


    //}

    //async void JoinRelay(string joinCode)
    //{
        //try
        //{
        //    Debug.Log("Joining relay : " + joinCode);
        //    JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);


        //    NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
        //        joinAllocation.RelayServer.IpV4,
        //        (ushort)joinAllocation.RelayServer.Port,
        //        joinAllocation.AllocationIdBytes,
        //        joinAllocation.Key,
        //        joinAllocation.ConnectionData,
        //        joinAllocation.HostConnectionData


        //        );
        //    NetworkManager.Singleton.StartClient();
        //    roomJoinCode.text = joinCode;
        //    codeInputField.SetActive(false);
        //    serverBtn.SetActive(false);
        //    clientBtn.SetActive(false);
        //    hostBtn.SetActive(false);


        //}
        //catch (RelayServiceException exception)
        //{
        //    Debug.Log(exception.Message);
        //}

    //}

    public void JoinRelayHelper()
    {
        //string joinCode = joinCodeInput.text;
        //JoinRelay(joinCode);
    }



    // Update is called once per frame
    void Update()
    {

    }
}

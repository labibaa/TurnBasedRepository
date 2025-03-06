using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;

public class TemporaryStats : MonoBehaviour, IPersistableData
{
    public int CurrentHealth;
    public int PlayerHealth;
    public GameObject Pathmark;
    public int CurrentAP;
    public int PlayerAP; 
    public int CurrentDex;
    public int CurrentEndurance;
    public int CurrentStrength;
    public int CurrentArcana;
    public int CurrentIntelligence;
    public int CurrentExp;
    public int CurrentResolve;
    public float CurrentDamageMultiplier;
    public bool IsBlockActive;
    public bool IsDodgeActive; 
    public bool IsThirdRatePerformanceActive;
    public bool IsCounterActive;
    public bool AutoMove;
    public int playerUltimateBarCount=0;
    public GameObject PlayerUltimateBar;

    public bool isMainCharacter;
    public bool isLinkOn;
    public String currentScene;
    public Mortality playerMortality;
    
    public Vector3 currentPlayerGridPosition;
    [SerializeField]
    Vector3 _spawnPosition;
    [SerializeField]
    Vector2 gridCoordinateSpawn;

    public GameObject lastPosition;
  
    public Image playerStatPanel;
    public GameObject PlayerActionListPanel;
    CharacterBaseClasses _characterBaseClasses;

    public Transform FlyingTextParent;
    CinemachineVirtualCamera playerCam;

    public TeamName CharacterTeam;
    public GameObject SelectionParticle;
    public GameObject EnemyTargetSelectionParticle;
    public int playerVisiblity=1;


    //Animator animator;

    private void Awake()
    {
        _characterBaseClasses = GetComponent<CharacterBaseClasses>();
        // animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {

        //GridSystem.OnGridGenerationSpawn += AssignSpawnPosition;
       // HealthManager.OnGridDisable += onEndFunction;
        WaveManager.OnGridInit += SetWeaponActions;
        WaveManager.OnGridInit += onEndFunction;
        ExperienceManager.instance.OnExperienceChanged += HandleExperienceChange;

    }

    private void OnDisable()
    {
        
        //GridSystem.OnGridGenerationSpawn -= AssignSpawnPosition;
       // HealthManager.OnGridDisable -= onEndFunction;
        WaveManager.OnGridInit -= SetWeaponActions;
        WaveManager.OnGridInit -= onEndFunction;
        ExperienceManager.instance.OnExperienceChanged -= HandleExperienceChange;
    }


    private void Start()
    {

        SetWeaponActions();
        onEndFunction();
        //s
        // Call the function MyFunction after one second
        //Invoke("AssignPosition", 1.0f);
        currentScene = SceneManager.GetActiveScene().name;
    }

    public void SaveData(PlayerDataSave playerDataSave)
    {
        playerDataSave.Name = _characterBaseClasses.name;
        playerDataSave.CurrentPlayerHealth = this.CurrentHealth;
        playerDataSave.CurrentExp = this.CurrentExp;
        playerDataSave.PlayerAP = this.PlayerAP;
        playerDataSave.CurrentDex = _characterBaseClasses.Dexterity;
        playerDataSave.CurrentStrength = _characterBaseClasses.Strength;
        playerDataSave.CurrentEndurance = _characterBaseClasses.Endurance;
        playerDataSave.CurrentArcana = _characterBaseClasses.Arcana;
        playerDataSave.CurrentIntelligence = _characterBaseClasses.Intelligence;
        playerDataSave.CharacterTeam = this.CharacterTeam;
    }

    public void LoadData(PlayerDataSave playerDataSave)
    {
        _characterBaseClasses.name = playerDataSave.Name ;
         this.CurrentHealth = playerDataSave.CurrentPlayerHealth ;
         this.CurrentExp = playerDataSave.CurrentExp ;
        this.PlayerAP = playerDataSave.PlayerAP;
        _characterBaseClasses.Dexterity = playerDataSave.CurrentDex ;
        _characterBaseClasses.Strength = playerDataSave.CurrentStrength;
        _characterBaseClasses.Endurance = playerDataSave.CurrentEndurance;
        _characterBaseClasses.Arcana = playerDataSave.CurrentArcana;
        _characterBaseClasses.Intelligence = playerDataSave.CurrentIntelligence;
        this.CharacterTeam = playerDataSave.CharacterTeam;

    }
    public void SetWeaponActions()
    {
       if(_characterBaseClasses.EquipedWeapon == CurrentWeapon.Dagger)
        {
           _characterBaseClasses.SetAvailableActions ( WeaponManager.instance.GetDaggerAvailableActions());
        }
        if (_characterBaseClasses.EquipedWeapon == CurrentWeapon.Sword)
        {
            _characterBaseClasses.SetAvailableActions( WeaponManager.instance.GetSwordAvailableActions());
        }
        if (_characterBaseClasses.EquipedWeapon == CurrentWeapon.BowAndArrow)
        {
           _characterBaseClasses.SetAvailableActions( WeaponManager.instance.GetBowAndArrowAvailableActions());
        }
        if (_characterBaseClasses.EquipedWeapon == CurrentWeapon.Talisman)
        {
            _characterBaseClasses.SetAvailableActions(WeaponManager.instance.GetTalismanAvailableActions());
        }
        if (_characterBaseClasses.EquipedWeapon == CurrentWeapon.Hammer)
        {
            _characterBaseClasses.SetAvailableActions(WeaponManager.instance.GetHammerAvailableActions());
        }
        if (_characterBaseClasses.EquipedWeapon == CurrentWeapon.Axe)
        {
            _characterBaseClasses.SetAvailableActions(WeaponManager.instance.GetAxeAvailableActions());
        }
        if (_characterBaseClasses.EquipedWeapon == CurrentWeapon.Spear)
        {
            _characterBaseClasses.SetAvailableActions(WeaponManager.instance.GetSpearAvailableActions());
        }
        if (_characterBaseClasses.EquipedWeapon == CurrentWeapon.Staff)
        {
            _characterBaseClasses.SetAvailableActions(WeaponManager.instance.GetStaffAvailableActions());
        }
    }
    public void SetCharacterStat()
    {
        PlayerHealth = _characterBaseClasses.HealthPoints; 
        CurrentDex = _characterBaseClasses.Dexterity;
        CurrentStrength = _characterBaseClasses.Strength;
        CurrentIntelligence = _characterBaseClasses.Intelligence;
        CurrentEndurance = _characterBaseClasses.Endurance;
        CurrentArcana = _characterBaseClasses.Arcana;
        CurrentDamageMultiplier = _characterBaseClasses.DamageMultiplier;


    }

    void onEndFunction()
    {
       // currentPlayerGridPosition = transform.position;
       
        

        PlayerActionListPanel = ButtonStackManager.instance.PopulateActionPanel(_characterBaseClasses);
        PlayerUltimateBar = ButtonStackManager.instance.PopulateUltimateBar(_characterBaseClasses);
        PlayerActionListPanel.SetActive(false);
        PlayerUltimateBar.SetActive(false);
    }

   
    private void HandleExperienceChange(int newExp)
    {
        CurrentExp += newExp;
       /* if(CurrentExp >= _characterBaseClasses.MaxExperiencePoint)
        {
            _characterBaseClasses.LevelUp();
        }*/
    }
    public void AssignSpawnPosition()
    {

        SetCharacterStat();

       // animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash);

        if ( gridCoordinateSpawn.x< GridSystem.instance._gridArray.GetLength(0) && gridCoordinateSpawn.x < GridSystem.instance._gridArray.GetLength(1))
        {
           

            transform.position = GridSystem.instance._gridArray[(int)gridCoordinateSpawn.x, (int)gridCoordinateSpawn.y].transform.position;
        }

        
        currentPlayerGridPosition = transform.position;
        transform.position = currentPlayerGridPosition;


        Debug.Log(this.name + " hh: "+ currentPlayerGridPosition + " hh222 " + transform.position);
        playerMortality = Mortality.Alive;
        OrbSpawner.instance.PlayerReadyCounter();
        ResetStatForGrid();
        TeamManager.instance.AddPlayerToTeamList(this);


        GridPlayerAnimation gridPlayerAnimation;
        if (TryGetComponent<GridPlayerAnimation>(out gridPlayerAnimation))
        {
            // Call the SetMoveAnimation method
            
            gridPlayerAnimation.SetMoveAnimation(0, 1);
        }

    }

    private void ResetStatForGrid()
    {
        IsBlockActive = false;
        IsDodgeActive = false;
        AutoMove = false;
        playerVisiblity = 1;
        playerUltimateBarCount = 0;
        playerMortality = Mortality.Alive;
        GetComponent<PlayerTurn>().isMoveOn = true;
        CurrentAP = PlayerAP;
        CurrentHealth = PlayerHealth;

    }

    public void SelectTarget()
    {
        if (GridSystem.instance.IsGridOn)
        {

            if(TempManager.instance.currentState == GameStates.TargetSelectionTurn && TurnManager.instance.targetsInRange.Contains(_characterBaseClasses))
            {
                TempManager.instance.defender = gameObject;
                //ActionArchive.instance.GetPlayerStats();
                TempManager.instance.ChangeGameState(GameStates.MidTurn);
                DictionaryManager.instance.GiveAction(TempManager.instance.actionName);

                //DictionaryManager.instance.GiveAction(TempManager.instance.actionName).Invoke();
                GridMovement.instance.ResetHighlightedPath();
                TurnManager.instance.ResetTargetHIghlightVisual();
                TurnManager.instance.targetsInRange.Clear();
                TurnManager.instance.nonCharacterTargetsInRange.Clear();
            }

            //UI.instance.ShowPanel(playerStatPanel);
            //playerStatPanel.gameObject.SetActive(true);
            //PlayerStatUI.instance.GetPlayerStatDetails(GetComponent<CharacterBaseClasses>());
        }
    }

 
}

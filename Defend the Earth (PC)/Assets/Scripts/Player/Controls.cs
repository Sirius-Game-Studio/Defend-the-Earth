// GENERATED AUTOMATICALLY FROM 'Assets/Controls.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Controls : IInputActionCollection
{
    private InputActionAsset asset;
    public Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""d19a9177-375a-46b9-9c97-fa9345d27f59"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""2ba4f28d-580e-4a42-bc06-9094fd358b0f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""4a4bde00-5f97-4d68-892f-d5ff9337a896"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""44cddc41-d935-43f6-a5b3-5e03b3fab229"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b203e9e4-1602-408e-8805-9132ae925947"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0f935123-bbba-41c4-8ef2-f42a3cc407ee"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92dced37-2271-4504-a67b-b6f0c0b181ab"",
                    ""path"": ""<Joystick>/trigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""192f7335-f544-4c0d-a1ad-1ae74384174d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3b4938ef-101d-476d-b595-095c191e6fea"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ee2ed389-a2d7-404a-81b8-83f54ee56b28"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5799f759-ba7c-4dc7-9e67-75c2b1ce46ff"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b2c9340f-42c5-4a0e-81dd-fd6b46201b77"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""a43e442a-0529-44c3-ad5d-a2ae44b1b43e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""0d85766e-cf14-4fa3-80c5-82d8b7373417"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""cbabd2ca-e96a-40cf-aae4-8bb800fa5e9b"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""4f05120f-2cca-4b23-9575-d32d31482c65"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3727a00f-e90b-40a0-8665-18a9adf0fcae"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Analog Stick"",
                    ""id"": ""c5600742-a43c-4dee-bf87-6f74de0b1717"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""27589569-1a64-47c8-a5de-fcb822ca5121"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c6bd405b-c0fe-4d5c-9dc0-19cc99c16e2f"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""652da4d4-60c6-49ca-8313-9defc7854d2d"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f86c50de-5851-4d1b-a872-559e1e1b2bb9"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""D-Pad"",
                    ""id"": ""ba42c167-8b75-4218-996b-3efff9843393"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f7ba55d1-197b-46fb-af3a-55586aa84ea7"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0e39fe76-2091-4aa7-b12a-cc3258073c7c"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""11edbdab-000b-43f0-808c-80295bef083e"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""17349211-eadb-47b4-b315-968911c862fc"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Joystick"",
                    ""id"": ""f6f45bac-b116-4e38-98e4-ec11527c9d81"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c7a2f36d-18c0-45b4-ad22-d5a25dc99c56"",
                    ""path"": ""<Joystick>/stick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""818748fd-c352-4157-9d7b-ecf90eb27561"",
                    ""path"": ""<Joystick>/stick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d1e24262-ed52-4873-91ce-23c4668af210"",
                    ""path"": ""<Joystick>/stick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""57e83122-2387-4312-84f4-4161812d80df"",
                    ""path"": ""<Joystick>/stick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Gameplay"",
            ""id"": ""26fb9768-2f21-43b1-a1b5-5ca4b05c8a0c"",
            ""actions"": [
                {
                    ""name"": ""Fullscreen"",
                    ""type"": ""Button"",
                    ""id"": ""03625aca-ecf4-43d7-a9ed-1f2dd46fde65"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""71670120-61b5-40f5-9a39-f92cca42a6c1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Resume"",
                    ""type"": ""Button"",
                    ""id"": ""e1bccf02-5f1e-4bd1-9087-4bec0ec8bfb1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Restart"",
                    ""type"": ""Button"",
                    ""id"": ""eba9d825-6ad8-43ae-8381-bc7658141b22"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""660a4d14-d7ea-4554-837f-33d14328e9ec"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""59b3c983-446b-403b-af5a-89589f16b0c0"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e7d9150a-74df-4671-b7c9-6c6e2ce8f2d8"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Restart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cfc35b33-9714-4d22-bcca-dcc4a154d247"",
                    ""path"": ""<Keyboard>/f11"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Fullscreen"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4a0e3702-3940-46b8-9f27-eb18b8d5034c"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Resume"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""b373f2b5-c71c-48ef-a94b-f061aec5f7bc"",
            ""actions"": [
                {
                    ""name"": ""BuySpaceship"",
                    ""type"": ""Button"",
                    ""id"": ""d660bd35-035a-4b37-b3f2-0e826eb525b0"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpaceshipsLeft"",
                    ""type"": ""Button"",
                    ""id"": ""b5487503-f746-40ab-84f5-842e48c23e42"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpaceshipsRight"",
                    ""type"": ""Button"",
                    ""id"": ""d1aad275-b4d6-4781-bd5b-731be2570690"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CloseMenu"",
                    ""type"": ""Button"",
                    ""id"": ""d4a702e3-3b11-4be9-beea-b66454e84fc3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpeedUpCredits"",
                    ""type"": ""Button"",
                    ""id"": ""571029e2-5578-4ef9-9a8f-217b35ebfa78"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8de2256f-03fa-4d7d-8e13-8bf6d6476ec6"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SpaceshipsLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e646f503-4297-4755-bbab-3989836dc0a7"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""BuySpaceship"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d273a9b-772b-47c0-a9f6-f22a8814abbd"",
                    ""path"": ""<Joystick>/trigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""BuySpaceship"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e0277a4c-36fd-444e-b46c-62eafdcb5337"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""CloseMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6caa4495-4e9c-42ba-92ca-7fa20c92165c"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""CloseMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54870bf6-4ed9-4cce-8772-b1ac64ed47b9"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""SpeedUpCredits"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""53c9005d-31b4-4d44-b179-4895184da913"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SpeedUpCredits"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""14136928-7fa0-43a8-b891-9d321b21d3ee"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SpaceshipsRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Debug"",
            ""id"": ""b9eaf0e9-b3a6-4dbc-826e-9bcc178c18c2"",
            ""actions"": [
                {
                    ""name"": ""SmallRepair"",
                    ""type"": ""Button"",
                    ""id"": ""4aec2f8e-e438-463f-98f7-7285c08e56f3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LargeRepair"",
                    ""type"": ""Button"",
                    ""id"": ""a0d663df-05cc-4787-afcb-159a181feec4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MaxHealth"",
                    ""type"": ""Button"",
                    ""id"": ""a571205a-278a-4579-ad42-4acc22b49c7f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Supercharge"",
                    ""type"": ""Button"",
                    ""id"": ""b09b6a5e-e19d-4f31-85ed-a1c231ab676b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NextWave"",
                    ""type"": ""Button"",
                    ""id"": ""9b949834-344e-409b-961a-059c86a64528"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SkipToBoss"",
                    ""type"": ""Button"",
                    ""id"": ""b1d99540-fe61-447b-8900-a80c5b48114e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9260e02a-1682-4817-b539-045951965f93"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""SmallRepair"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f4e60ea9-9522-41aa-ada7-f16d282add4a"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SmallRepair"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46099c3f-4ac3-47bd-aa71-ae0b72d01a8d"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""LargeRepair"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a8ea6d16-d7e6-4d2f-97df-83ec0a45ad2d"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""LargeRepair"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b79b50bd-71c5-4e21-87f9-5d54a490e2c5"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""MaxHealth"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8530e089-e058-4074-a064-34cd69231479"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MaxHealth"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3d06e7cf-cdd7-4901-b092-7177e0b78f25"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Supercharge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6c9ce740-bb37-4798-a45d-603921e58e62"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Supercharge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""033c63a6-e4bf-4063-9636-670144b2196d"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""NextWave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""80873819-088a-4ebf-8513-6d3243ee884a"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""NextWave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2decb665-4cb6-45f6-891d-d727c5bde056"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""SkipToBoss"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b5269d75-8181-4d2d-8ba7-02812c083c0d"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SkipToBoss"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Mouse"",
            ""bindingGroup"": ""Keyboard & Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Joystick"",
            ""bindingGroup"": ""Joystick"",
            ""devices"": [
                {
                    ""devicePath"": ""<Joystick>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Fire = m_Player.FindAction("Fire", throwIfNotFound: true);
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Fullscreen = m_Gameplay.FindAction("Fullscreen", throwIfNotFound: true);
        m_Gameplay_Pause = m_Gameplay.FindAction("Pause", throwIfNotFound: true);
        m_Gameplay_Resume = m_Gameplay.FindAction("Resume", throwIfNotFound: true);
        m_Gameplay_Restart = m_Gameplay.FindAction("Restart", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_BuySpaceship = m_Menu.FindAction("BuySpaceship", throwIfNotFound: true);
        m_Menu_SpaceshipsLeft = m_Menu.FindAction("SpaceshipsLeft", throwIfNotFound: true);
        m_Menu_SpaceshipsRight = m_Menu.FindAction("SpaceshipsRight", throwIfNotFound: true);
        m_Menu_CloseMenu = m_Menu.FindAction("CloseMenu", throwIfNotFound: true);
        m_Menu_SpeedUpCredits = m_Menu.FindAction("SpeedUpCredits", throwIfNotFound: true);
        // Debug
        m_Debug = asset.FindActionMap("Debug", throwIfNotFound: true);
        m_Debug_SmallRepair = m_Debug.FindAction("SmallRepair", throwIfNotFound: true);
        m_Debug_LargeRepair = m_Debug.FindAction("LargeRepair", throwIfNotFound: true);
        m_Debug_MaxHealth = m_Debug.FindAction("MaxHealth", throwIfNotFound: true);
        m_Debug_Supercharge = m_Debug.FindAction("Supercharge", throwIfNotFound: true);
        m_Debug_NextWave = m_Debug.FindAction("NextWave", throwIfNotFound: true);
        m_Debug_SkipToBoss = m_Debug.FindAction("SkipToBoss", throwIfNotFound: true);
    }

    ~Controls()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Fire;
    public struct PlayerActions
    {
        private Controls m_Wrapper;
        public PlayerActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Fire => m_Wrapper.m_Player_Fire;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                Fire.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                Fire.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
                Fire.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                Move.started += instance.OnMove;
                Move.performed += instance.OnMove;
                Move.canceled += instance.OnMove;
                Fire.started += instance.OnFire;
                Fire.performed += instance.OnFire;
                Fire.canceled += instance.OnFire;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Fullscreen;
    private readonly InputAction m_Gameplay_Pause;
    private readonly InputAction m_Gameplay_Resume;
    private readonly InputAction m_Gameplay_Restart;
    public struct GameplayActions
    {
        private Controls m_Wrapper;
        public GameplayActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Fullscreen => m_Wrapper.m_Gameplay_Fullscreen;
        public InputAction @Pause => m_Wrapper.m_Gameplay_Pause;
        public InputAction @Resume => m_Wrapper.m_Gameplay_Resume;
        public InputAction @Restart => m_Wrapper.m_Gameplay_Restart;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                Fullscreen.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFullscreen;
                Fullscreen.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFullscreen;
                Fullscreen.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnFullscreen;
                Pause.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                Pause.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                Pause.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                Resume.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResume;
                Resume.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResume;
                Resume.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnResume;
                Restart.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRestart;
                Restart.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRestart;
                Restart.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRestart;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                Fullscreen.started += instance.OnFullscreen;
                Fullscreen.performed += instance.OnFullscreen;
                Fullscreen.canceled += instance.OnFullscreen;
                Pause.started += instance.OnPause;
                Pause.performed += instance.OnPause;
                Pause.canceled += instance.OnPause;
                Resume.started += instance.OnResume;
                Resume.performed += instance.OnResume;
                Resume.canceled += instance.OnResume;
                Restart.started += instance.OnRestart;
                Restart.performed += instance.OnRestart;
                Restart.canceled += instance.OnRestart;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_BuySpaceship;
    private readonly InputAction m_Menu_SpaceshipsLeft;
    private readonly InputAction m_Menu_SpaceshipsRight;
    private readonly InputAction m_Menu_CloseMenu;
    private readonly InputAction m_Menu_SpeedUpCredits;
    public struct MenuActions
    {
        private Controls m_Wrapper;
        public MenuActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @BuySpaceship => m_Wrapper.m_Menu_BuySpaceship;
        public InputAction @SpaceshipsLeft => m_Wrapper.m_Menu_SpaceshipsLeft;
        public InputAction @SpaceshipsRight => m_Wrapper.m_Menu_SpaceshipsRight;
        public InputAction @CloseMenu => m_Wrapper.m_Menu_CloseMenu;
        public InputAction @SpeedUpCredits => m_Wrapper.m_Menu_SpeedUpCredits;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                BuySpaceship.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnBuySpaceship;
                BuySpaceship.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnBuySpaceship;
                BuySpaceship.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnBuySpaceship;
                SpaceshipsLeft.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSpaceshipsLeft;
                SpaceshipsLeft.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSpaceshipsLeft;
                SpaceshipsLeft.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSpaceshipsLeft;
                SpaceshipsRight.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSpaceshipsRight;
                SpaceshipsRight.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSpaceshipsRight;
                SpaceshipsRight.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSpaceshipsRight;
                CloseMenu.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnCloseMenu;
                CloseMenu.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnCloseMenu;
                CloseMenu.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnCloseMenu;
                SpeedUpCredits.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSpeedUpCredits;
                SpeedUpCredits.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSpeedUpCredits;
                SpeedUpCredits.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSpeedUpCredits;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                BuySpaceship.started += instance.OnBuySpaceship;
                BuySpaceship.performed += instance.OnBuySpaceship;
                BuySpaceship.canceled += instance.OnBuySpaceship;
                SpaceshipsLeft.started += instance.OnSpaceshipsLeft;
                SpaceshipsLeft.performed += instance.OnSpaceshipsLeft;
                SpaceshipsLeft.canceled += instance.OnSpaceshipsLeft;
                SpaceshipsRight.started += instance.OnSpaceshipsRight;
                SpaceshipsRight.performed += instance.OnSpaceshipsRight;
                SpaceshipsRight.canceled += instance.OnSpaceshipsRight;
                CloseMenu.started += instance.OnCloseMenu;
                CloseMenu.performed += instance.OnCloseMenu;
                CloseMenu.canceled += instance.OnCloseMenu;
                SpeedUpCredits.started += instance.OnSpeedUpCredits;
                SpeedUpCredits.performed += instance.OnSpeedUpCredits;
                SpeedUpCredits.canceled += instance.OnSpeedUpCredits;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);

    // Debug
    private readonly InputActionMap m_Debug;
    private IDebugActions m_DebugActionsCallbackInterface;
    private readonly InputAction m_Debug_SmallRepair;
    private readonly InputAction m_Debug_LargeRepair;
    private readonly InputAction m_Debug_MaxHealth;
    private readonly InputAction m_Debug_Supercharge;
    private readonly InputAction m_Debug_NextWave;
    private readonly InputAction m_Debug_SkipToBoss;
    public struct DebugActions
    {
        private Controls m_Wrapper;
        public DebugActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @SmallRepair => m_Wrapper.m_Debug_SmallRepair;
        public InputAction @LargeRepair => m_Wrapper.m_Debug_LargeRepair;
        public InputAction @MaxHealth => m_Wrapper.m_Debug_MaxHealth;
        public InputAction @Supercharge => m_Wrapper.m_Debug_Supercharge;
        public InputAction @NextWave => m_Wrapper.m_Debug_NextWave;
        public InputAction @SkipToBoss => m_Wrapper.m_Debug_SkipToBoss;
        public InputActionMap Get() { return m_Wrapper.m_Debug; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DebugActions set) { return set.Get(); }
        public void SetCallbacks(IDebugActions instance)
        {
            if (m_Wrapper.m_DebugActionsCallbackInterface != null)
            {
                SmallRepair.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnSmallRepair;
                SmallRepair.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnSmallRepair;
                SmallRepair.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnSmallRepair;
                LargeRepair.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnLargeRepair;
                LargeRepair.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnLargeRepair;
                LargeRepair.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnLargeRepair;
                MaxHealth.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnMaxHealth;
                MaxHealth.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnMaxHealth;
                MaxHealth.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnMaxHealth;
                Supercharge.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnSupercharge;
                Supercharge.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnSupercharge;
                Supercharge.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnSupercharge;
                NextWave.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnNextWave;
                NextWave.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnNextWave;
                NextWave.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnNextWave;
                SkipToBoss.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnSkipToBoss;
                SkipToBoss.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnSkipToBoss;
                SkipToBoss.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnSkipToBoss;
            }
            m_Wrapper.m_DebugActionsCallbackInterface = instance;
            if (instance != null)
            {
                SmallRepair.started += instance.OnSmallRepair;
                SmallRepair.performed += instance.OnSmallRepair;
                SmallRepair.canceled += instance.OnSmallRepair;
                LargeRepair.started += instance.OnLargeRepair;
                LargeRepair.performed += instance.OnLargeRepair;
                LargeRepair.canceled += instance.OnLargeRepair;
                MaxHealth.started += instance.OnMaxHealth;
                MaxHealth.performed += instance.OnMaxHealth;
                MaxHealth.canceled += instance.OnMaxHealth;
                Supercharge.started += instance.OnSupercharge;
                Supercharge.performed += instance.OnSupercharge;
                Supercharge.canceled += instance.OnSupercharge;
                NextWave.started += instance.OnNextWave;
                NextWave.performed += instance.OnNextWave;
                NextWave.canceled += instance.OnNextWave;
                SkipToBoss.started += instance.OnSkipToBoss;
                SkipToBoss.performed += instance.OnSkipToBoss;
                SkipToBoss.canceled += instance.OnSkipToBoss;
            }
        }
    }
    public DebugActions @Debug => new DebugActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_JoystickSchemeIndex = -1;
    public InputControlScheme JoystickScheme
    {
        get
        {
            if (m_JoystickSchemeIndex == -1) m_JoystickSchemeIndex = asset.FindControlSchemeIndex("Joystick");
            return asset.controlSchemes[m_JoystickSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
    }
    public interface IGameplayActions
    {
        void OnFullscreen(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnResume(InputAction.CallbackContext context);
        void OnRestart(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
        void OnBuySpaceship(InputAction.CallbackContext context);
        void OnSpaceshipsLeft(InputAction.CallbackContext context);
        void OnSpaceshipsRight(InputAction.CallbackContext context);
        void OnCloseMenu(InputAction.CallbackContext context);
        void OnSpeedUpCredits(InputAction.CallbackContext context);
    }
    public interface IDebugActions
    {
        void OnSmallRepair(InputAction.CallbackContext context);
        void OnLargeRepair(InputAction.CallbackContext context);
        void OnMaxHealth(InputAction.CallbackContext context);
        void OnSupercharge(InputAction.CallbackContext context);
        void OnNextWave(InputAction.CallbackContext context);
        void OnSkipToBoss(InputAction.CallbackContext context);
    }
}

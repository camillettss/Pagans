using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public interface UIController
{
    void onSubmit(InputAction.CallbackContext ctx);

    void onCancel(InputAction.CallbackContext ctx);

    void onNavigate(InputAction.CallbackContext ctx);

}

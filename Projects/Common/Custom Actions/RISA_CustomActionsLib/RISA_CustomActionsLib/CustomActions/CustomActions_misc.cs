using System;
using Microsoft.Deployment.WindowsInstaller;
using RISA_CustomActionsLib.Models;

namespace RISA_CustomActionsLib
{
    public partial class CustomActions
    {

        #region Misc functions

        private static void copySinglePropFromSession(SessionDTO sessDTO, Session session, string propName, bool propMustExist = true)
        {
            // the whole idea is to provide enough info (property name) should an installer fail to establish a required prop
            try
            {
                sessDTO[propName] = session[propName];
            }
            catch (Exception ex)
            {
                if (!propMustExist) return;
                throw new IndexOutOfRangeException($"Error retrieving Session property {propName}", ex);
            }
        }

        private static void setupDebugIfRequested(Session session, SessionDTO sessDTO)
        {

            copySinglePropFromSession(sessDTO, session, _propRISA_CA_DEBUG, false);
            if (!sessDTO.PropertyExists(_propRISA_CA_DEBUG)) return;
            switch (sessDTO[_propRISA_CA_DEBUG])
            {
                case _debug_Trace:
                    _doTrace = true;
                    break;
                // other debug types here
                default:
                    break;
            }
        }

        #endregion
    }
}

using NITGEN.SDK.NBioBSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rdpServiceSocket
{
    class utilsNitgen
    {

        NBioAPI m_NBioAPI = new NBioAPI();

       

        public string Capturar()
        {
            

            NBioAPI.Type.HFIR hCapturedFIR = new NBioAPI.Type.HFIR();
            NBioAPI.Type.HFIR hCapturedFIRaudit = new NBioAPI.Type.HFIR();
            NBioAPI.Type.FIR_TEXTENCODE texto = new NBioAPI.Type.FIR_TEXTENCODE();
            //Get FIR data
            uint device = m_NBioAPI.OpenDevice(255);
          
            if (device != NBioAPI.Error.NONE)
            {
                return null;
                
            }
            m_NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.VERIFY, out hCapturedFIR, -1, hCapturedFIRaudit, null);
            m_NBioAPI.GetTextFIRFromHandle(hCapturedFIRaudit, out texto, true);
            m_NBioAPI.CloseDevice(255);
            return texto.TextFIR;

        }

        public string Enroll()
        {
            
            NBioAPI.Type.FIR_TEXTENCODE m_textFIR = new NBioAPI.Type.FIR_TEXTENCODE();
            NBioAPI.Type.HFIR NewFIR = new NBioAPI.Type.HFIR();          
            NBioAPI.Type.HFIR NewFIRoudit = new NBioAPI.Type.HFIR();          

            NBioAPI.Type.WINDOW_OPTION m_WinOption = new NBioAPI.Type.WINDOW_OPTION();
            m_WinOption.WindowStyle = (uint)NBioAPI.Type.WINDOW_STYLE.NO_WELCOME;
            string Retorno = "";            
            uint device = m_NBioAPI.OpenDevice(255);
            if (device != NBioAPI.Error.NONE)
            {
                return null;

            }


            uint ret = m_NBioAPI.Enroll(null, out NewFIR, null, -1 ,NewFIRoudit, m_WinOption);
            //uint ret = m_NBioAPI.Enroll(null, out NewFIR, null, NBioAPI.Type.TIMEOUT.DEFAULT, null, m_WinOption);
           
            if (NewFIR != null)
            {
                m_NBioAPI.GetTextFIRFromHandle(NewFIRoudit, out m_textFIR, true);

                if (m_textFIR.TextFIR != null)
                {
                    m_NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
                    Retorno = m_textFIR.TextFIR.ToString();
                }
            }
            m_NBioAPI.CloseDevice(255);
            return Retorno;
        }
    }
}


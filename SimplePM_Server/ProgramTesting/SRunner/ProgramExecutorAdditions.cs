using CompilerPlugin;
using System.Security;
using System.Diagnostics;
using SimplePM_Server.Workers;

namespace SimplePM_Server.ProgramTesting.SRunner
{
    
    public class ProgramExecutorAdditions
    {
        
        public static void SetExecInfoByFileExt(
            ref dynamic languageConfiguration,
            ref ICompilerPlugin _compilerPlugin,
            ref ProcessStartInfo startInfo,
            string filePath,
            string arguments
        )
        {
            
            /*
             * Вызываем метод, отвечающий за установку
             * дополнительных параметров запуска.
             */
            var f = _compilerPlugin.SetRunningMethod(
                ref languageConfiguration,
                ref startInfo,
                filePath
            );
            
            // В случае возникновения ошибок выбрасываем исключение
            if (!f)
                throw new SimplePM_Exceptions.UnknownException("SetRunningMethod() failed!");

            // Обрабатываем аргументы запуска
            if (startInfo.Arguments.Length > 0)
                startInfo.Arguments += " " + arguments;
            else
                startInfo.Arguments = arguments;

        }
        
        public static void SetProcessRunAs(ref Process proc)
        {

            /*
             * Проверяем, включена  ли  функция  запуска
             * пользовательских программ от имени инного
             * пользователя. Если отключена - выходим.
             */

            if ((string)(SWorker._securityConfiguration.runas.enabled) != "true")
                return;

            // Указываем имя пользователя
            proc.StartInfo.UserName = (string)(SWorker._securityConfiguration.runas.username);

            /*
             * Передаём, что загружать пользова-
             * тельский профайл не нужно.
             */

            proc.StartInfo.LoadUserProfile = false;

            /*
             * Передаём пароль пользователя
             */

            // Создаём защищённую строку
            var encPassword = new SecureString();

            // Добавляем данные в защищённую строку
            foreach (var c in (string)(SWorker._securityConfiguration.runas.password))
                encPassword.AppendChar(c);

            // Устанавливаем пароль пользователя
            proc.StartInfo.Password = encPassword;
            
        }
        
    }
    
}
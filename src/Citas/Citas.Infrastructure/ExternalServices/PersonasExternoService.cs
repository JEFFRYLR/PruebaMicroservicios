using Citas.Domain.DTOs;
using Citas.Domain.Interfaces;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Citas.Infrastructure.ExternalServices
{
    /// <summary>
    /// Implementación del cliente HTTP para comunicarse con el microservicio de Personas
    /// </summary>
    public class PersonasExternoService : IPersonasExternoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _personasApiBaseUrl;
        private string _bearerToken;  //  NUEVO

        public PersonasExternoService(string baseUrl = null)
        {
            // Asegurar que la URL base NO termine con barra
            _personasApiBaseUrl = (baseUrl ?? "http://localhost:11947/api/personas").TrimEnd('/');

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //  NUEVO: Método para configurar el token
        public void SetAuthorizationToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _bearerToken = token;
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
                
                System.Diagnostics.Debug.WriteLine(
                    "Token JWT configurado en PersonasExternoService");
            }
        }

        public async Task<bool> ExisteMedicoAsync(int medicoId)
        {
            try
            {
                var persona = await ObtenerPersonaPorIdAsync(medicoId);
                
                if (persona == null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        string.Format("Persona con ID {0} no encontrada", medicoId));
                    return false;
                }

                // TipoPersona: 1 = Paciente, 2 = Medico
                bool esMedico = persona.TipoPersona == 2;
                
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Persona {0}: Nombre={1}, TipoPersona={2}, EsMedico={3}", 
                    medicoId, persona.Nombre, persona.TipoPersona, esMedico));
                
                return esMedico;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Error validando médico {0}: {1}", 
                        medicoId, ex.Message));
                throw new InvalidOperationException(
                    string.Format("No se pudo validar el médico con ID {0}. " +
                        "El servicio de Personas no está disponible.", medicoId), ex);
            }
        }

        public async Task<bool> ExistePacienteAsync(int pacienteId)
        {
            try
            {
                var persona = await ObtenerPersonaPorIdAsync(pacienteId);
                
                if (persona == null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        string.Format("Persona con ID {0} no encontrada", pacienteId));
                    return false;
                }

                // TipoPersona: 1 = Paciente, 2 = Medico
                bool esPaciente = persona.TipoPersona == 1;
                
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Persona {0}: Nombre={1}, TipoPersona={2}, EsPaciente={3}", 
                    pacienteId, persona.Nombre, persona.TipoPersona, esPaciente));
                
                return esPaciente;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Error validando paciente {0}: {1}", 
                        pacienteId, ex.Message));
                throw new InvalidOperationException(
                    string.Format("No se pudo validar el paciente con ID {0}. " +
                        "El servicio de Personas no está disponible.", pacienteId), ex);
            }
        }

        public async Task<PersonaExternaDto> ObtenerPersonaPorIdAsync(int personaId)
        {
            try
            {
                // Construir URL completa manualmente
                var url = string.Format("{0}/{1}", _personasApiBaseUrl, personaId);
                
                System.Diagnostics.Debug.WriteLine(
                    string.Format("=== CONSULTANDO PERSONAS API ==="));
                System.Diagnostics.Debug.WriteLine(
                    string.Format("URL: {0}", url));
                
                //  El token ya está configurado en los headers
                // por el método SetAuthorizationToken()
                
                var response = await _httpClient.GetAsync(url);

                System.Diagnostics.Debug.WriteLine(
                    string.Format("Response StatusCode: {0}", response.StatusCode));

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    System.Diagnostics.Debug.WriteLine(
                        string.Format("Persona {0} no encontrada (404)", personaId));
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Response Content: {0}", content));

                var persona = JsonConvert.DeserializeObject<PersonaExternaDto>(content);

                System.Diagnostics.Debug.WriteLine(
                    string.Format("Persona deserializada: ID={0}, Nombre={1}, TipoPersona={2}", 
                    persona.Id, persona.Nombre, persona.TipoPersona));

                return persona;
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("=== ERROR HTTP ==="));
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Message: {0}", httpEx.Message));
                
                throw new InvalidOperationException(
                    "No se pudo conectar con el servicio de Personas. " +
                    "Verifique que esté disponible.", httpEx);
            }
            catch (TaskCanceledException timeoutEx)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("=== TIMEOUT ==="));
                
                throw new InvalidOperationException(
                    "El servicio de Personas no respondió a tiempo.", timeoutEx);
            }
            catch (JsonException jsonEx)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("=== ERROR JSON ==="));
                
                throw new InvalidOperationException(
                    "Error al procesar la respuesta del servicio de Personas.", jsonEx);
            }
        }
    }
}
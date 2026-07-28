# 1. Gestión de médicos

### Historia
**Como clínica, quiero registrar un médico, para que pueda ofrecer citas en la plataforma.**

**Versión mínima aceptable**
Permitir registrar un médico ingresando únicamente su nombre completo.

**Criterios de aceptación**

- La clínica puede registrar un nuevo médico.
- El nombre del médico es obligatorio.
- El médico registrado queda disponible para asociarle una especialidad.
- El médico aparece en el listado de médicos.

---

# 2. Asociación de especialidad

### Historia
**Como clínica, quiero asociar una especialidad a un médico, para que los pacientes puedan encontrarlo al buscar una especialidad.**

**Versión mínima aceptable**
Permitir seleccionar una especialidad existente y asociarla a un médico.

**Criterios de aceptación**

- La clínica puede seleccionar una especialidad de un listado predefinido.
- Un médico debe tener una especialidad asociada.
- El paciente puede encontrar al médico al buscar esa especialidad.
- No se permite guardar un médico sin especialidad.

---

# 3. Configuración de horarios

### Historia
**Como clínica, quiero configurar los horarios disponibles de cada médico, para que los pacientes puedan reservar citas.**

**Versión mínima aceptable**
Permitir definir los días y horarios disponibles para un médico.

**Criterios de aceptación**

- La clínica puede asignar horarios disponibles a un médico.
- Solo es posible configurar horarios de lunes a viernes.
- No es posible configurar horarios para sábados ni domingos.
- Los horarios configurados quedan disponibles para los pacientes.

---

# 4. Visualización de citas

### Historia
**Como clínica, quiero visualizar las citas programadas, para administrar la agenda de atención.**

**Versión mínima aceptable**
Mostrar un listado de todas las citas registradas.

**Criterios de aceptación**

- La clínica puede visualizar todas las citas programadas.
- Cada cita muestra el nombre del paciente.
- Cada cita muestra el médico asignado.
- Cada cita muestra la fecha y hora programadas.

---

# 5. Búsqueda de médicos

### Historia
**Como paciente, quiero buscar médicos por especialidad, para elegir con quién reservar una cita.**

**Versión mínima aceptable**
Permitir seleccionar una especialidad y mostrar los médicos asociados.

**Criterios de aceptación**

- El paciente puede seleccionar una especialidad.
- El sistema muestra únicamente los médicos de esa especialidad.
- Si no existen médicos para la especialidad seleccionada, el sistema informa que no hay disponibilidad.

---

# 6. Consulta de disponibilidad

### Historia
**Como paciente, quiero consultar los horarios disponibles de un médico, para seleccionar una fecha de atención.**

**Versión mínima aceptable**
Mostrar los horarios disponibles del médico seleccionado.

**Criterios de aceptación**

- El paciente puede consultar la disponibilidad de un médico.
- Solo se muestran horarios disponibles de lunes a viernes.
- No se muestran horarios ocupados.
- Si el médico no tiene horarios disponibles, el sistema informa la situación.

---

# 7. Reserva de cita

### Historia
**Como paciente, quiero reservar una cita en un horario disponible, para asegurar mi atención médica.**

**Versión mínima aceptable**
Permitir seleccionar un horario disponible y confirmar la reserva.

**Criterios de aceptación**

- El paciente puede seleccionar un horario disponible.
- El sistema registra la reserva correctamente.
- Solo se pueden reservar citas de lunes a viernes.
- Un horario solo puede ser reservado por un paciente.
- Una vez realizada la reserva, el horario deja de estar disponible para otros pacientes.

---

# 8. Confirmación de reserva

### Historia
**Como paciente, quiero recibir una confirmación inmediata de la reserva, para saber que mi cita quedó registrada.**

**Versión mínima aceptable**
Mostrar un mensaje de confirmación al finalizar la reserva.

**Criterios de aceptación**

- El sistema muestra un mensaje de confirmación al finalizar la reserva.
- La confirmación incluye el nombre del médico.
- La confirmación incluye la fecha y hora de la cita.
- La confirmación se muestra únicamente cuando la reserva fue registrada exitosamente.

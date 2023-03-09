extends Spatial

export var spherical_position: Vector3 setget set_spherical_position
var is_set_spherical_position := false
export var speed: float = 1.0
export var acceleration_factor: float = 0.5

var spherical_velocity := Vector3.ZERO

func _ready():
	pass

func _process(delta):
	var position_change := Vector3.ZERO
	if Input.is_action_pressed("move_forward"):
		position_change.x -= 1
	if Input.is_action_pressed("move_backward"):
		position_change.x += 1 
	if Input.is_action_pressed("move_up"):
		position_change.z += PI / self.spherical_position.x
	if Input.is_action_pressed("move_down"):
		position_change.z -= PI / self.spherical_position.x
	if Input.is_action_pressed("move_left"):
		position_change.y -= (
			PI 
			/ self.spherical_position.x 
			/ (cos(self.spherical_position.z) + 1)
		)
	if Input.is_action_pressed("move_right"):
		position_change.y += (
			PI 
			/ self.spherical_position.x 
			/ (cos(self.spherical_position.z) + 1)
		)
	
	self.spherical_velocity = lerp(
		self.spherical_velocity
		, position_change
		, acceleration_factor
	)
	
	self.spherical_position += self.spherical_velocity * self.speed * delta
	
	self.spherical_position.x = max(self.spherical_position.x, 4)
	self.spherical_position.z = clamp(self.spherical_position.z, -PI/2, PI/2)

func set_spherical_position(position: Vector3):
	spherical_position = position
	self.transform.origin = to_cartesian(position)
	
	# look_at cause error when not onready
	if not is_set_spherical_position:
		is_set_spherical_position = true
		return
		
	look_at(
		Vector3.ZERO
		, Vector3.UP
	)

# spherical_vector = (r, theta, phi)
func to_cartesian(spherical_vector: Vector3) -> Vector3:
	return spherical_vector.x * Vector3(
		cos(spherical_vector.z)*sin(spherical_vector.y)
		, sin(spherical_vector.z)
		, cos(spherical_vector.z)*cos(spherical_vector.y)
	)

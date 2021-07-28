using Sandbox;
using System;
using System.Linq;

public partial class Player
{
	[ServerCmd( "explode", Help = "Explode yourself" )]
	public static void Explode()
	{
		var target = ConsoleSystem.Caller;
		var p = target.Pawn as Player;
		if ( p != null )
		{
			p.TakeDamage( DamageInfo.FromBullet( p.PhysicsBody.MassCenter, Vector3.Zero, p.Health ).WithFlag( DamageFlags.Blast ) );
		}
	}

	public override void OnKilled()
	{
		base.OnKilled();

		Inventory.DeleteContents();

		Log.Info( LastDamage.Flags );
		if ( (LastDamage.Flags & DamageFlags.Blast) != 0 ) GibOnClient();
		else BecomeRagdollOnClient( LastDamage.Force, GetHitboxBone( LastDamage.HitboxIndex ) );

		RemoveClothes();
		RemoveAllDecals();

		Controller = null;
		Camera = new SpectateRagdollCamera();

		EnableAllCollisions = false;
		EnableDrawing = false;

		Event.Run( "player_killed", new KillArgs( LastDamage, this ) );
	}

	// TODO - make ragdolls one per entity
	// TODO - make ragdolls dissapear after a load of seconds
	static EntityLimit RagdollLimit = new EntityLimit { MaxTotal = 10 };
	static EntityLimit GibLimit = new EntityLimit { MaxTotal = 30 };

	[ClientRpc]
	void GibOnClient()
	{
		var pos = Position + Vector3.Up * 32;
		Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", pos );
		Particles.Create( "particles/impact.flesh-big.vpcf", pos );
		PlaySound( "kersplat" );

		var head_ragdoll = new ModelEntity();
		head_ragdoll.Position = Position;
		head_ragdoll.Rotation = Rotation;
		head_ragdoll.MoveType = MoveType.Physics;
		head_ragdoll.UsePhysicsCollision = true;
		head_ragdoll.SetInteractsAs( CollisionLayer.Debris );
		head_ragdoll.SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
		head_ragdoll.SetInteractsExclude( CollisionLayer.Player | CollisionLayer.Debris );

		head_ragdoll.SetModel( "models/citizen/head_ragdoll.vmdl" );
		//head_ragdoll.SetMaterialGroup( GetMaterialGroup() );
		//head_ragdoll.CopyBonesFrom( this );
		head_ragdoll.TakeDecalsFrom( this );
		head_ragdoll.SetRagdollVelocityFrom( this );
		head_ragdoll.DeleteAsync( 20.0f );
		head_ragdoll.PhysicsGroup.AddVelocity( Vector3.Random * 100 );
		// Crown
		var crown = Children.FirstOrDefault( c => c.Tags.Has( "crown" ) ) as ModelEntity;
		if ( crown != null )
		{
			crown = new ModelEntity( crown.GetModelName() );
			crown.SetParent( head_ragdoll, true );
		}
		GibLimit.Watch( head_ragdoll );


		for ( int i = 0; i < 10; i++ )
		{
			var mdl = new string[]
			{
				"models/sbox_props/watermelon/watermelon_gib07.vmdl",
				"models/sbox_props/watermelon/watermelon_gib08.vmdl",
				"models/sbox_props/watermelon/watermelon_gib09.vmdl",
				"models/sbox_props/watermelon/watermelon_gib06.vmdl"
			}.GetRandom();

			var gib = new Prop();
			gib.SetModel( mdl );
			gib.RenderColor = new ColorHsv( 0, 0, 0.5f ).ToColor();
			gib.Position = Position + Vector3.Up * 32;
			gib.Velocity = ((Vector3.Up * 0.5f) + Vector3.Random) * 200;
			gib.Scale = 1 + (float)new Random().NextDouble() * 3f;
			gib.DeleteAsync( 20 );

			GibLimit.Watch( gib );
		}

		Corpse = head_ragdoll;
	}

	[ClientRpc]
	void BecomeRagdollOnClient( Vector3 force, int forceBone )
	{
		// TODO - lets not make everyone write this shit out all the time
		// maybe a CreateRagdoll<T>() on ModelEntity?
		var ent = new ModelEntity();

		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.MoveType = MoveType.Physics;
		ent.UsePhysicsCollision = true;
		ent.SetInteractsAs( CollisionLayer.Debris );
		ent.SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
		ent.SetInteractsExclude( CollisionLayer.Player | CollisionLayer.Debris );

		ent.SetModel( GetModelName() );
		ent.SetMaterialGroup( GetMaterialGroup() );
		ent.CopyBonesFrom( this );
		ent.TakeDecalsFrom( this );
		ent.SetRagdollVelocityFrom( this );
		ent.DeleteAsync( 20.0f );

		// Copy the clothes over
		foreach ( var child in Children )
		{
			if ( child is ModelEntity e )
			{
				if ( !e.Tags.Has( "clothes" ) )
					continue;

				if ( e.Tags.Has( "crown" ) && forceBone == GetBoneIndex( "head" ) ) continue;

				var clothing = new ModelEntity( e.GetModelName() );
				clothing.SetParent( ent, true );
			}
		}

		if ( forceBone == GetBoneIndex( "head" ) )
		{

			var head_ragdoll = new ModelEntity();
			head_ragdoll.Position = Position;
			head_ragdoll.Rotation = Rotation;
			head_ragdoll.MoveType = MoveType.Physics;
			head_ragdoll.UsePhysicsCollision = true;
			head_ragdoll.SetInteractsAs( CollisionLayer.Debris );
			head_ragdoll.SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
			head_ragdoll.SetInteractsExclude( CollisionLayer.Player | CollisionLayer.Debris );

			head_ragdoll.SetModel( "models/citizen/head_ragdoll.vmdl" );
			//head_ragdoll.SetMaterialGroup( GetMaterialGroup() );
			//head_ragdoll.CopyBonesFrom( this );
			head_ragdoll.TakeDecalsFrom( this );
			head_ragdoll.SetRagdollVelocityFrom( this );
			head_ragdoll.DeleteAsync( 20.0f );
			head_ragdoll.PhysicsGroup.AddVelocity( force * 10 );
			GibLimit.Watch( head_ragdoll );

			var crown = Children.FirstOrDefault( c => c.Tags.Has( "crown" ) ) as ModelEntity;
			if ( crown != null )
			{
				crown = new ModelEntity( crown.GetModelName() );
				crown.SetParent( head_ragdoll, true );
			}

			ent.SetBodyGroup( 0, 1 );
			var headbone = new ModelEntity( "models/citizen/head_bone.vmdl" );
			headbone.SetParent( ent, true );

			Plane p = new Plane( ent.PhysicsBody.MassCenter, Vector3.Up );
			var m = ent.GetModel();
			var verts = m.GetVertices();

		}

		ent.PhysicsGroup.AddVelocity( force );

		if ( forceBone >= 0 )
		{
			var body = ent.GetBonePhysicsBody( forceBone );
			if ( body != null )
			{
				body.ApplyForce( force * 1000 );
			}
			else
			{
				ent.PhysicsGroup.AddVelocity( force );
			}
		}


		Corpse = ent;

		RagdollLimit.Watch( ent );
	}

}

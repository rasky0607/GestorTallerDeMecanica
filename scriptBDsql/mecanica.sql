create database if not exists mecanica character set='utf8';
	use mecanica;

drop table if exists reparacion;
drop table if exists servicio;
drop table if exists coche;
drop table if exists cliente;


	create table cliente(
		id integer AUTO_INCREMENT,
		dni char(9) UNIQUE,
		nombre varchar(200),
		apellidos varchar(200),
		tlf integer,
		primary key(id)
		)engine=InnoDB;

	create table coche(
		matricula varchar(10),
		marca varchar(200),
		modelo varchar(200),
		primary key(matricula)
		)engine=InnoDB;

	create table servicio(
		codigo integer,
		descripcion varchar(255),
		precio decimal(6,2),
		primary key(codigo)
		);

	create table reparacion(
		id integer,
		idCliente integer,
		matriCoche varchar(10),
		codServicio integer,
		fecha date,
		primary key(id,idCliente,matriCoche,codServicio,fecha),
		foreign key (idCliente) references cliente (id) on update cascade on delete restrict,
		foreign key (matriCoche) references coche (matricula) on update cascade on delete restrict,
		foreign key (codServicio) references servicio (codigo) on update cascade on delete restrict
		);

		insert into cliente values(1,'12345678A','Francisco','Lanzat','638023928');
		insert into cliente values(2,'87654321B','Pablo','Lopez','634023227');
		insert into cliente values(3,'12365478C','Oscar','Castro','628025178');

		insert into coche values('2218CL','NISSAN','SERENA');
		insert into coche values('1700JPG','OPEL','CORSA');
		insert into coche values('8792FLC','FORD','MUSTANG');

		insert into servicio values(1,'Mano de obra',15);
		insert into servicio values(2,'aceite',20);
		insert into servicio values(3,'filtro',20);
		insert into servicio values(4,'neumatico',55);
		insert into servicio values(5,'Pastillas de freno',30);
		insert into servicio values(6,'Liquido de freno',25);


		 insert into reparacion values(1,1,'2218CL',4,'2019-06-17');
		 insert into reparacion values(2,1,'2218CL',4,'2019-06-17');
		 insert into reparacion values(3,1,'2218CL',4,'2019-06-17');
		 insert into reparacion values(4,1,'2218CL',4,'2019-06-17');
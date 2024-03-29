create database if not exists mecanica character set='utf8';
	use mecanica;

drop table if exists factura;
drop table if exists reparacion;
drop table if exists servicio;
drop table if exists coche;
drop table if exists cliente;


	create table cliente(
		idCliente integer AUTO_INCREMENT not null,		
		nombre varchar(250) not null,
		apellidos text default null,
		tlf integer not null,
		matricula varchar(20),
		marca varchar(250) default null,
		modelo varchar(250) default null,
		primary key(idCliente,matricula)
		);
	create table servicio(
		codigo integer AUTO_INCREMENT primary key,
		descripcion varchar(250) UNIQUE not null,
		precio decimal(6,2) default null
		);

	create table reparacion(
		numReparacion integer,
		idCliente integer,
		matriCoche varchar(20),
		codServicio integer,
		fecha date,
		estadoReparacion enum('FACTURADA','NO FACTURADA') default 'NO FACTURADA',
		primary key(numReparacion,idCliente,matriCoche,fecha),
		foreign key (idCliente,matriCoche) references cliente (idCliente,matricula) on update cascade on delete restrict,
		foreign key (codServicio) references servicio (codigo) on update cascade on delete restrict
		);
/*Linea representa el numReparacion de reparacion*/
create table factura(
		numeroFactura integer,
		linea integer,
		idCliente integer,
		matriCoche varchar(20),
		codServicio integer,
		fecha date,
		estadoFactura enum('ANULADA','VIGENTE') default 'VIGENTE',
		numeroFacturaAnulada integer default 0,		
		primary key(numeroFactura,linea) -- idCliente,matriCoche,fecha
		/*foreign key (codServicio) references reparacion (codServicio) on update cascade on delete restrict,
		foreign key (linea,idCliente,matriCoche,fecha) references reparacion (numReparacion,idCliente,matriCoche,fecha) on update cascade on delete restrict*/
		
	);

		insert into cliente values(1,'Francisco','Lanzat','638023928','2218CL','Nissan','SERENA');
		insert into cliente values(2,'Pablo','Lopez','634023227','1700JPG','OPEL','Corsa');
		insert into cliente values(3,'Oscar','Castro','628025178','2019OPL','Suzuki','MIERDER');

		insert into servicio values(1,'Mano de obra',15);
		insert into servicio values(2,'aceite',20);
		insert into servicio values(3,'filtro',20);
		insert into servicio values(4,'neumatico',55);
		insert into servicio values(5,'Pastillas de freno',30);
		insert into servicio values(6,'Liquido de freno',25);

		insert into reparacion values(1,1,'2218CL',4,'2019-06-01','FACTURADA');
		insert into reparacion values(2,1,'2218CL',3,'2019-06-01','FACTURADA');
		insert into reparacion values(3,1,'2218CL',4,'2019-06-01','FACTURADA');
		insert into reparacion values(1,2,'1700JPG',5,'2019-02-21','FACTURADA');
		insert into reparacion values(1,3,'2019OPL',6,'2019-03-19','NO FACTURADA');
		insert into reparacion values(1,1,'2218CL',6,'2019-07-11','NO FACTURADA');
		insert into reparacion values(2,1,'2218CL',6,'2019-07-11','NO FACTURADA');


		
		insert into factura values(1,1,1,'2218CL',4,'2019-06-01','VIGENTE',NULL);
		insert into factura values(1,2,1,'2218CL',3,'2019-06-01','VIGENTE',NULL);
		insert into factura values(1,3,1,'2218CL',4,'2019-06-01','VIGENTE',NULL); 
		insert into factura values(2,1,2,'1700JPG',5,'2019-02-21','VIGENTE',NULL);
			

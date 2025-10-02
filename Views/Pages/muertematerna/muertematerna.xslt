<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:user="http://mycompany.com/mynamespace">

  <msxsl:script language="C#" implements-prefix="user" >
    public string crearCeldas(string sCadena, int iCeldas){

    string sHtml = "";
    sCadena = string.IsNullOrEmpty(sCadena) ? "" : sCadena;
    int iLetras = sCadena.Length;
    char[] lstLetras = sCadena.ToCharArray();

    for (int i = 0; i <![CDATA[<]]> iCeldas; i++)
    {

    if(iCeldas <![CDATA[>]]> iLetras <![CDATA[&&]]> i <![CDATA[>=]]> iLetras <![CDATA[||]]> lstLetras.Length<![CDATA[==]]> 0 ){


    sHtml += "<![CDATA[<]]>td<![CDATA[>]]>&#160;<![CDATA[<]]>/td<![CDATA[>]]>";

    } else {

    sHtml += "<![CDATA[<]]>td<![CDATA[>]]>" + lstLetras[i] + "<![CDATA[<]]>/td<![CDATA[>]]>";

    }

    }

    sHtml = "<![CDATA[<]]>table<![CDATA[>]]><![CDATA[<]]>tr<![CDATA[>]]>" + sHtml + "<![CDATA[<]]>/tr<![CDATA[>]]><![CDATA[<]]>/table<![CDATA[>]]>";
    return sHtml;

    }
  </msxsl:script>

  <xsl:template match="Cuerpo">
    <html>
      <head>
        <style>
            html{font-size: 10pt; font-family: Arial; }


            #div_table_Parte_A{ background-image: url("#UrlDelSitio#/Registros/Varicela831/DatosBasicos.jpg"); background-repeat: no-repeat; background-size: 100% 100%; }


            .cssDivCasillas { margin: 0px auto; padding: 0; vertical-align: middle; text-align: center; }
            .cssDivCasillas table{vertical-align: middle; text-align: center; text-align: center; width:100%;}

            span{ position: absolute; font-weight: bold; }
            .posotionAbsolute{ position: absolute; font-weight: bold; }
            .cssTextoDoc { font-size: 15pt;}
            .cssTextoPeq { font-size: 6pt; position: absolute; }
            .cssTextoMed { font-size: 10pt; position: absolute; }
            .topCodigoUpgd{top: 253px;}

            #CodigoUpgd1 {left: 40px; width:50px;}
            #CodigoUpgd2 {left: 110px; width:75px;}
            #CodigoUpgd3 {left: 194px; width:125px;}
            #CodigoUpgd4 {left: 330px; width:50px;}
            #div_RazonSocial {left: 410px; top: 253px;}

            .topLiena2{top: 310px;}

            #div_NombreEvento {left: 35px;}
            #div_CodigoEvento {left: 450px; width:75px;}
            #div_FechaEvento1 {left: 558px; width:50px;}
            #div_FechaEvento2 {left: 621px; width:50px;}
            #div_FechaEvento3 {left: 691px; width:145px;}

            .topLiena3{top: 339px; font-size: 12pt; }
            #div_TipoIdentificacionRC {left: 33px;}
            #div_TipoIdentificacionTI {left: 82px;}
            #div_TipoIdentificacionCC {left: 123px;}
            #div_TipoIdentificacionCE {left: 171px;}
            #div_TipoIdentificacionPA {left: 223px;}
            #div_TipoIdentificacionMS {left: 273px;}
            #div_TipoIdentificacionAS {left: 332px;}
            #div_TipoIdentificacionPE {left: 384px;}
            #div_Identificacion {left: 430px;}

            .topLinea4{top: 403px; font-size 9pt;}
            #div_NombrePaciente{left:50px;}
            #div_TelefonoPaciente{left:729px;}

            .topLinea5{top: 451px;}
            #div_fechaNaciemiento  {left: 29px; width:50px;}
            #div_fechaNaciemiento2 {left: 94px; width:55px;}
            #div_fechaNaciemiento3 {left: 163px; width:105px;}
            #div_edadPaciente {left: 285px;}
            #div_TipoMedidaA {left: 350px;}

            .topLinea1P2{top:1550px;}
            #div_TipoIdentificacionp2{left:490px;}
            #div_TipoIdentificacionp3{left:570px;}

            .topLineaP23{top: 1716px}
            #div_grupoSintomas1{left: 440px}
            #div_grupoSintomas2{left: 565px}
            #div_grupoSintomas3{left: 740px}

            .topLineaP24{top: 1743px}
            #div_grupoSintomas4{left: 440px}
            #div_grupoSintomas5{left: 565px}


            .topLineaP25{top: 1815px}
            #div_grupoAnimales1{left: 280px}
            #div_grupoAnimales2{left: 418px}
            #div_grupoAnimales3{left: 550px}

            .topLineaP26{top: 1843px}
            #div_grupoAnimales4{left: 280px}
            #div_grupoAnimales5{left: 418px}

            .topLineaP27{top: 1870px}
            #div_grupoAnimales6{left: 280px}
            #div_grupoAnimales7{left: 418px}

            .topLineaP28{top: 1860px}
            #div_grupoAnimalesOtros{left: 665px}

            .topLineaP29{top: 1932px}
            #div_vistaderatasSi{left: 468px}

            .topLineaP30{top: 1930px}
            #div_vistaderatasNo{left: 468px}

            .topLineaP31{top: 2005px}
            #div_grupoAbastecimientoAgua1{left: 70px}
            #div_grupoAbastecimientoAgua2{left: 230px}

            .topLineaP32{top: 2030px}
            #div_grupoAbastecimientoAgua3{left: 70px}
            #div_grupoAbastecimientoAgua4{left: 230px}

            .topLineaP33{top: 2010px}
            #div_AlcantarilladoSi{left: 568px}

            .topLineaP34{top: 2033px}
            #div_AlcantarilladoNo{left: 568px}


            .topLineaP35{top: 2080px}
            #div_AguaResidualSi{left: 290px}

            .topLineaP36{top: 2105px}
            #div_AguaResidualNo{left: 290px}


            .topLineaP37{top: 2112px}
            #div_grupoActividadesRecreativas1{left: 450px}
            #div_grupoActividadesRecreativas2{left: 586px}
            #div_grupoActividadesRecreativas3{left: 725px}

            .topLineaP38{top: 2140px}
            #div_grupoActividadesRecreativas4{left: 448px}
            #div_grupoActividadesRecreativas5{left: 586px}

            .topLineaP39{top: 2185px}
            #div_DisposiciondeResiduos1{left: 290px}

            .topLineaP40{top: 2220px}
            #div_DisposiciondeResiduos2{left: 292px}

            .topLinea2P2{top:1645px;}
            .topLinea2_1P2{top:1662px;}
            #div_L21{left: 121px;}
            #div_L22{left: 600px; width:90px;}
            #div_L23{left: 710px; width:50px;}
            #div_L24{left: 780px; width:100px;}

            .topLinea3P2{top:1699px}
            #div_L25{left:567px;}
            #div_L26{left:653px;}
            #div_L27{left:760px;}


            .topLinea4P2{top: 1727px}
            #div_L28{left:410px;}



            .topLinea5P2{top: 1851px}
            .topLinea5_1P2{top: 1866px}
            .topLinea5_2P2{top: 1881px}
            .topLinea5_3P2{top: 1896px}
            .topLinea5_4P2{top: 1911px}
            .topLinea5_5P2{top: 1926px}
            #div_L29{left:57px;}

            .topLinea6P2{top: 1840px}

            #div_L30{left:249px;}
            .topLinea6_1P2{top: 1871px}
            .topLinea6_2P2{top: 1886px}
            .topLinea6_3P2{top: 1901px}
            .topLinea6_4P2{top: 1916px}
            .topLinea6_5P2{top: 1931px}

            #div_L31{left:445px;}

            #div_L32{left:663px;}
            .topLinea7P2{top: 1845px}
            .topLinea7_1P2{top: 1860px}
            .topLinea7_2P2{top: 1890px}
            .topLinea7_3P2{top: 1905px}


            .topLinea8P2{top: 2006px}
            #div_L33{left:200px;}
            #div_L34{left:314px;}
            #div_L35{left:527px;}
            #div_L36{left:740px;}

            .topLinea9P2{top: 2034px}
            .topLinea9_1P2{top: 2048px}
            #div_L37{left:149px;}
            #div_L38{left:277px;}
            #div_L39{left:464px;}
            #div_L40{left:674px;}

            .topLinea10P2{top: 2105px}
            .topLinea10_1P2{top: 2120px}
            #div_L41{left:301px;}
            #div_L42{left:399px;}
            #div_L43{left:507px;}
            #div_L44{left:607px;}

            .topLinea11P2{top: 2215px}
            #div_L45{left:50px; width: 50px;}
            #div_L46{left:120px; width: 50px;}
            #div_L47{left:185px; width: 120px;}

            #div_L48{left:340px; width: 50px;}
            #div_L49{left:410px; width: 50px;}
            #div_L50{left:470px; width: 120px;}
            #div_L51{left:670px;}
            #div_L52{left:745px;}
            #div_L53{left:830px;}


            .topLinea12P2{top: 2270px}
            #div_L54{left:180px; width: 50px;}
            #div_L55{left:248px; width: 50px;}
            #div_L56{left:315px; width: 120px;}
            #div_L57{left:515px;}


            .topLinea5_2{top: 448px}
            #div_sexoM{left: 580px }
            #div_sexoI{left: 661px }

            .topLinea5_3{top: 468px}
            #div_sexoF{left: 580px }

            .topLinea5_4{top: 431px}
            #div_nacionalidad{left: 840px; width:60px}

            .topLinea5_4N{top: 473px}
            #div_nacionalidadNombre{left: 760px}

            .topLinea6{top: 503px}
            #div_paisOcurrencia{left: 237px; width:73px}

            .topLinea6_N{top: 523px}
            #div_paisOcurrenciaNombre{left: 27px}

            .topLinea6_2{top: 513px}
            #div_departamentoOcurrencia{left: 523px; width:50px}

            #div_municipioOcurrencia{left: 592px; width:73px}

            .topLinea6_2N{top: 523px}
            #div_departamentoOcurrenciaNombre{left: 323px}

            .topLinea6_3{top: 508px}
            #div_areaOcurrencia1{left: 678px}
            #div_areaOcurrencia3{left: 814px}

            .topLinea6_4{top: 526px}
            #div_areaOcurrencia2{left: 678px}


            .topLinea7{top: 563px}
            #div_localidadOcurrencia{left: 34px}
            #div_barrioOcurrencia{left: 280px}
            #div_cabeceraOcurrencia{left: 470px}
            #div_veredaOcurrencia{left: 724px}

            .topLinea8{top: 618px}
            #div_ocupacionPaciente{left: 210px; width: 90px}
            .topLinea8N{top: 622px}
            #div_ocupacionPacienteNombre{left: 10px; width: 200px}

            .topLinea8_2{top: 614px}
            #div_tipoRegimen1{left: 316px}
            #div_tipoRegimen2{left: 391px}
            #div_tipoRegimen3{left: 475px}

            .topLinea8_3{top: 634px}
            #div_tipoRegimen4{left: 316px}
            #div_tipoRegimen5{left: 391px}
            #div_tipoRegimen6{left: 475px}

            .topLinea8_4{top: 619px}
            #div_Administradora{left: 752px; width: 145px}
            #div_AdministradoraNombre{left: 607px; width: 125px}

            .topLinea9{top: 667px}
            #div_pertenenciaEtnica1{left: 155px}
            #div_pertenenciaEtnicaOtroCual{left: 228px}
            #div_pertenenciaEtnica2{left: 355px}
            #div_pertenenciaEtnica3{left: 448px}
            #div_pertenenciaEtnica4{left: 525px}
            #div_pertenenciaEtnica5{left: 617px}
            #div_pertenenciaEtnica6{left: 784px}
            #div_estrato{left: 838px}

            .topLinea10{top: 716px}
            #div_grupoPoblacion1{left: 30px}
            #div_grupoPoblacion2{left: 123px}
            #div_grupoPoblacion3{left: 204px}
            #div_grupoPoblacion4{left: 382px}
            #div_grupoPoblacion5{left: 594px}
            #div_grupoPoblacion6{left: 736px}


            .topLinea10_3{top: 723px}
            #div_grupoPoblacionSemanas{left: 280px}

            .topLinea11{top: 807px}
            #div_fuente1{left: 26px}
            #div_fuente2{left: 162px}

            .topLinea11_2{top: 823px}
            #div_fuente3{left: 26px}
            #div_fuente4{left: 162px}


            .topLinea11_3{top: 837px}
            #div_fuente5{left: 26px}

            .topLinea11_4{top: 818px}
            #div_paisNotificacion{left: 673px; width: 75px}
            #div_paisNotificacionNombre{left: 250px; width: 275px}
            #div_departamentoNotificacion{left: 758px; width: 50px}
            #div_municipioNotificacion{left: 823px; width: 75px}

            .topLinea12{top: 859px}
            #div_direccionResidencia{left: 177px}

            .topLinea13{top: 915px}

            #div_fechaConsulta  {left: 34px; width:50px;}
            #div_fechaConsulta1 {left: 90px; width:55px;}
            #div_fechaConsulta2 {left: 160px; width:155px;}

            #div_fechaInicioSintomas  {left: 304px; width:50px;}
            #div_fechaInicioSintomas1 {left: 356px; width:55px;}
            #div_fechaInicioSintomas2 {left: 425px; width:155px;}

            .topLinea13_1{top: 905px}
            #div_clasificacionInicial{left: 555px}
            #div_clasificacionInicial3{left: 640px}

            .topLinea13_2{top: 921px}
            #div_clasificacionInicial2{left: 555px}
            #div_clasificacionInicial4{left: 640px}

            .topLinea13_3{top: 933px}
            #div_clasificacionInicial5{left: 640px}

            .topLinea13_4{top: 926px}
            #div_hospitalizado{left: 818px}

            .topLinea13_5{top: 923px}
            #div_hospitalizadoNO{left: 866px}

            .topLinea14{top: 993px}
            #div_fechaHospitalizacion  {left: 24px; width:50px;}
            #div_fechaHospitalizacion1 {left: 90px; width:55px;}
            #div_fechaHospitalizacion2 {left: 160px; width:105px;}

            .topLinea14_2{top: 976px}
            #div_CondicionFinal1{left: 276px}

            .topLinea14_3{top: 991px}
            #div_CondicionFinal2{left: 276px}
            #div_fechaDefuncion{left:  400px; width:50px;}
            #div_fechaDefuncion2{left: 460px; width:55px;}
            #div_fechaDefuncion3{left: 529px; width:105px;}
            #div_numeroCertificado{left: 659px}

            .topLinea14_4{top: 1006px}
            #div_CondicionFinal3{left: 276px}


            .topLinea15{top: 1054px}
            #div_nombreProfesional{left: 360px}
            #div_telefonoProfesional{left: 755px}

            .topLinea15_2{top: 1057px}
            #div_causaBasicaMuerte{left: 45px}

        </style>

      </head>
      <body >
        <!-- <div style='page-break-after:always;'>&#160;</div> ESTE ES EL SALTO A LA OTRA PAGINA-->
        <div style="width: 100%;height:1200px;background-color: blue;" id="div_table_Parte_A">&#160;</div>
        <div style='page-break-after:always;'>&#160;</div>

        <!--Linea 1-->
        <div  id="CodigoUpgd1" class="cssDivCasillas posotionAbsolute topCodigoUpgd">
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Nombre='Código de la UPGD']/@Valor,1,2),2)"/>
        </div>
        <!-- Campo faltante: Número de Registro -->
        <div id="div_NumeroRegistro" class="cssDivCasillas posotionAbsolute topCodigoUpgd" style="left: 500px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputNumeroRegistro']/@Valor"/>
        </div>

        <div  id="CodigoUpgd2" class="cssDivCasillas posotionAbsolute topCodigoUpgd" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Nombre='Código de la UPGD']/@Valor,4,3),3)"/>
        </div>

        <div  id="CodigoUpgd3" class="cssDivCasillas posotionAbsolute topCodigoUpgd"  >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Nombre='Código de la UPGD']/@Valor,8,5),5)"/>
        </div>

        <div  id="CodigoUpgd4" class="cssDivCasillas posotionAbsolute topCodigoUpgd"  >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Nombre='Código de la UPGD']/@Valor,14,2),2)"/>
        </div>

        <div  id="div_RazonSocial" class="cssDivCasillas posotionAbsolute topCodigoUpgd cssTextoDoc" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Razón social de la unidad primaria generadora del dato']/@Valor"/>
        </div>
        <!--FIN Linea 1-->

        <!--Linea 2-->
        <div  id="div_NombreEvento" class="cssDivCasillas posotionAbsolute topLiena2 cssTextoDoc" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Nombre del evento']/@Valor"/>
        </div>

        <div  id="div_CodigoEvento" class="cssDivCasillas posotionAbsolute topLiena2 cssTextoDoc" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputEvento']/@Valor,1,3),3)"/>

        </div>
          <div id="div_FechaEvento2" class="cssDivCasillas posotionAbsolute topLiena2">
              <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaHora']/@Valor,6,2), 2)"/>
          </div>
          <div id="div_FechaEvento1" class="cssDivCasillas posotionAbsolute topLiena2">
              <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaHora']/@Valor,9,4), 4)"/>
          </div>

          <div id="div_FechaEvento3" class="cssDivCasillas posotionAbsolute topLiena2">
              <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaHora']/@Valor,1,4), 8)"/>
          </div>
            <!-- Campo faltante: Formato -->
            <div id="div_Formato" class="cssDivCasillas posotionAbsolute topLiena2" style="left: 850px; width: 100px;">
              <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='fechaFormato']/@Valor"/>
            </div>
          

        
        <!--FIN Linea 2-->

        <!--Linea 3-->
        <!--TipoIdentificacion-->
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de documento']/@Identificador = 'tipoDocRC'">
          <div  id="div_TipoIdentificacionRC" class="cssDivCasillas posotionAbsolute topLiena3 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de documento']/@Identificador = 'tipoDocTI'">
          <div  id="div_TipoIdentificacionTI" class="cssDivCasillas posotionAbsolute topLiena3 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de documento']/@Identificador = 'tipoDocCC'">
          <div  id="div_TipoIdentificacionCC" class="cssDivCasillas posotionAbsolute topLiena3 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de documento']/@Identificador = 'tipoDocPE'">
          <div  id="div_TipoIdentificacionPE" class="cssDivCasillas posotionAbsolute topLiena3" >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de documento']/@Identificador = 'tipoDocCE'">
          <div  id="div_TipoIdentificacionCE" class="cssDivCasillas posotionAbsolute topLiena3" >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de documento']/@Identificador = 'tipoDocPA'">
          <div  id="div_TipoIdentificacionPA" class="cssDivCasillas posotionAbsolute topLiena3 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de documento']/@Identificador = 'tipoDocMS'">
          <div  id="div_TipoIdentificacionMS" class="cssDivCasillas posotionAbsolute topLiena3" >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de documento']/@Identificador = 'tipoDocAS'">
          <div  id="div_TipoIdentificacionAS" class="cssDivCasillas posotionAbsolute topLiena3" >
            X
          </div>
        </xsl:if>

        <div  id="div_Identificacion" class="cssDivCasillas posotionAbsolute topLiena3 cssTextoDoc" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Número de identificación']/@Valor"/>
        </div>
        <!--FIN Linea 3-->

        <!--INICIO Linea 4-->
        <div  id="div_NombrePaciente" class="cssDivCasillas posotionAbsolute topLinea4 cssTextoDoc" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Nombres y apellidos del paciente']/@Valor"/>
        </div>
        <div  id="div_TelefonoPaciente" class="cssDivCasillas posotionAbsolute topLinea4 cssTextoDoc" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Teléfono']/@Valor"/>
        </div>
        <!--FIN Linea 4-->

        <!--Inicio Linea 5-->
        <div  id="div_fechaNaciemiento" class="cssDivCasillas posotionAbsolute topLinea5" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaNaciemiento']/@Valor,1,2),2)"/>
        </div>

        <div  id="div_fechaNaciemiento2" class="cssDivCasillas posotionAbsolute topLinea5" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaNaciemiento']/@Valor,4,2),2)"/>
        </div>

        <div  id="div_fechaNaciemiento3" class="cssDivCasillas posotionAbsolute topLinea5" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaNaciemiento']/@Valor,7,4),4)"/>
        </div>
       

        <div  id="div_edadPaciente" class="cssDivCasillas posotionAbsolute topLinea5 cssTextoMed" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Edad']/@Valor"/>
        </div>
        <!-- Campo faltante: Fecha de nacimiento (oculta) -->
        <div id="div_FechaNacimientoOculta" class="cssDivCasillas posotionAbsolute topLinea5" style="left: 400px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='dtp_inputFechaNacimiento']/@Valor"/>
        </div>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Unidad de medida de la edad']/@Identificador = 'Años'">
          <div  id="div_TipoMedidaA" class="cssDivCasillas posotionAbsolute topLinea5 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Sexo']/@Identificador = 'Masculino'">
          <div  id="div_sexoM" class="cssDivCasillas posotionAbsolute topLinea5_2 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Sexo']/@Identificador = 'Indeterminado'">
          <div  id="div_sexoI" class="cssDivCasillas posotionAbsolute topLinea5_2 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Sexo']/@Identificador = 'Femenino'">
          <div  id="div_sexoF" class="cssDivCasillas posotionAbsolute topLinea5_3 " >
            X
          </div>
        </xsl:if>

        <div  id="div_nacionalidadNombre" class="cssDivCasillas posotionAbsolute topLinea5_4N" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputNacionalidadPaciente']/@ValorTexto"/>
        </div>
        <!-- Campo faltante: Nacionalidad (oculta) -->
        <div id="div_NacionalidadOculta" class="cssDivCasillas posotionAbsolute topLinea5_4N" style="left: 900px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputNacionalidadPaciente']/@Valor"/>
        </div>
        <div  id="div_nacionalidad" class="cssDivCasillas posotionAbsolute topLinea5_4" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputNacionalidadPaciente']/@ValorTexto,1,3),3)"/>
        </div>
        <!--FIN Linea 5-->

        <!--INICIO Linea 6-->
        <div  id="div_paisOcurrenciaNombre" class="cssDivCasillas posotionAbsolute topLinea6_N" >
          Colombia
        </div>
        <div  id="div_paisOcurrencia" class="cssDivCasillas posotionAbsolute topLinea6" >
          <table>
            <tr>
              <td>4</td>
              <td>7</td>
              <td>2</td>
            </tr>
          </table>
          <!-- <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputPaisOcurriencia']/@Valor,1,3),3)"/>-->
        </div>
        <div  id="div_departamentoOcurrenciaNombre" class="cssDivCasillas posotionAbsolute topLinea6_2N" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Departamento  procedencia/ocurrencia']/@ValorTexto"/>
        </div>
        <!-- Campo faltante: Municipio de ocurrencia -->
        <div id="div_MunicipioOcurrencia" class="cssDivCasillas posotionAbsolute topLinea6_2N" style="left: 500px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputMunicipioOcurriencia']/@Valor"/>
        </div>

        <div  id="div_departamentoOcurrencia" class="cssDivCasillas posotionAbsolute topLinea6_2" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputDepartamentoOcurriencia']/@Valor,1,2),2)"/>
        </div>
        <div  id="div_municipioOcurrencia" class="cssDivCasillas posotionAbsolute topLinea6_2" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputDepartamentoOcurriencia']/@Valor,3,3),3)"/>
        </div>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='AreaOcurrencia']/@Identificador = 'AOCM'">
          <div  id="div_areaOcurrencia1" class="cssDivCasillas posotionAbsolute topLinea6_3 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='AreaOcurrencia']/@Identificador = 'AORD'">
          <div  id="div_areaOcurrencia3" class="cssDivCasillas posotionAbsolute topLinea6_3 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='AreaOcurrencia']/@Identificador = 'AOCP'">
          <div  id="div_areaOcurrencia2" class="cssDivCasillas posotionAbsolute topLinea6_4 " >
            X
          </div>
        </xsl:if>
        <!--FIN Linea 6-->

        <!--INICIO Linea 7-->
        <div  id="div_localidadOcurrencia" class="cssDivCasillas cssTextoMed  topLinea7" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Localidad de ocurrencia del caso']/@Valor"/>
        </div>
        <div  id="div_barrioOcurrencia" class="cssDivCasillas cssTextoMed topLinea7" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Barrio de ocurrencia del caso']/@Valor"/>
        </div>

        <div  id="div_cabeceraOcurrencia" class="cssDivCasillas cssTextoMed topLinea7" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Cabecera municipal/centro poblado/rural disperso']/@Valor"/>
        </div>

        <div  id="div_veredaOcurrencia" class="cssDivCasillas cssTextoMed topLinea7" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Vereda/zona']/@Valor"/>
        </div>

        <!--FIN Linea 7-->

        <!--INICIO Linea 8-->
        <div  id="div_ocupacionPacienteNombre" class="cssDivCasillas cssTextoPeq topLinea8N" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputOcupacionPaciente']/@ValorTexto"/>
        </div>
        <div  id="div_ocupacionPaciente" class="cssDivCasillas posotionAbsolute topLinea8" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputOcupacionPaciente']/@Valor,1,4),4)"/>
        </div>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de régimen en salud']/@Identificador = 'Excepción'">
          <div  id="div_tipoRegimen1" class="cssDivCasillas posotionAbsolute topLinea8_2" >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de régimen en salud']/@Identificador = 'Contributivo'">
          <div  id="div_tipoRegimen2" class="cssDivCasillas posotionAbsolute topLinea8_2" >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de régimen en salud']/@Identificador = 'NoAsegurado'">
          <div  id="div_tipoRegimen3" class="cssDivCasillas posotionAbsolute topLinea8_2" >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de régimen en salud']/@Identificador = 'Especial'">
          <div  id="div_tipoRegimen4" class="cssDivCasillas posotionAbsolute topLinea8_3 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de régimen en salud']/@Identificador = 'Subsidiado'">
          <div  id="div_tipoRegimen5" class="cssDivCasillas posotionAbsolute topLinea8_3 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Tipo de régimen en salud']/@Identificador = 'IndeterminadoPendiente'">
          <div  id="div_tipoRegimen6" class="cssDivCasillas posotionAbsolute topLinea8_3 " >
            X
          </div>
        </xsl:if>
        <div  id="div_AdministradoraNombre" class="cssDivCasillas cssTextoPeq topLinea8_4" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Nombre de la administradora de Planes de beneficios']/@ValorTexto"/>
        </div>
        <!-- Campo faltante: Grupo Étnico -->
        <div id="div_GrupoEtnico" class="cssDivCasillas posotionAbsolute topLinea9" style="left: 900px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputGrupoEtnico']/@Valor"/>
        </div>
        <div  id="div_Administradora" class="cssDivCasillas posotionAbsolute topLinea8_4" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputADM']/@Valor,1,6),6)"/>
        </div>

        <!--FIN Linea 8-->


        <!-- LINEA 9-->
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Pertenencia étnica']/@Identificador = 'Indígena'">
          <div  id="div_pertenenciaEtnica1" class="cssDivCasillas posotionAbsolute topLinea9 " >
            X
          </div>
          <div  id="div_pertenenciaEtnicaOtroCual" class="cssDivCasillas posotionAbsolute topLinea9 " >
            <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Grupo Étnico']/@Valor"/>
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Pertenencia étnica']/@Identificador = 'RomGitano'">
          <div  id="div_pertenenciaEtnica2" class="cssDivCasillas posotionAbsolute topLinea9 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Pertenencia étnica']/@Identificador = 'Raizal'">
          <div  id="div_pertenenciaEtnica3" class="cssDivCasillas posotionAbsolute topLinea9 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Pertenencia étnica']/@Identificador = 'Palenquero'">
          <div  id="div_pertenenciaEtnica4" class="cssDivCasillas posotionAbsolute topLinea9 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Pertenencia étnica']/@Identificador = 'OtraPertenencia'">
          <div  id="div_pertenenciaEtnica6" class="cssDivCasillas posotionAbsolute topLinea9 " >
            X
          </div>
        </xsl:if>
        <div  id="div_estrato" class="cssDivCasillas posotionAbsolute topLinea9 cssTextoDoc" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Estrato']/@Valor"/>
        </div>
        <!-- LINEA 9 FIN-->

        <!-- LINEA 10 -->
        <xsl:if test="Bloques/Secciones/Campos[@Identificador='Discapacitados']/@Valor = 'True'">
          <div  id="div_grupoPoblacion1" class="cssDivCasillas posotionAbsolute topLinea10 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Identificador='Migrantes']/@Valor = 'True'">
          <div  id="div_grupoPoblacion2" class="cssDivCasillas posotionAbsolute topLinea10 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Identificador='Gestantes']/@Valor = 'True'">
          <div  id="div_grupoPoblacion3" class="cssDivCasillas posotionAbsolute topLinea10 " >
            X
          </div>
          <div  id="div_grupoPoblacionSemanas" class="cssDivCasillas posotionAbsolute topLinea10_3 " >
            <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Semanas']/@Valor"/>
          </div>
          <!-- Campo faltante: Estrato -->
          <div id="div_Estrato" class="cssDivCasillas posotionAbsolute topLinea10_3" style="left: 900px; width: 100px;">
            <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputEstrato']/@Valor"/>
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Identificador='PoblaciónICBF']/@Valor = 'True'">
          <div  id="div_grupoPoblacion4" class="cssDivCasillas posotionAbsolute topLinea10 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Identificador='Desmovilizados']/@Valor = 'True'">
          <div  id="div_grupoPoblacion5" class="cssDivCasillas posotionAbsolute topLinea10 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Identificador='VíctimasViolenciaArmada']/@Valor = 'True'">
          <div  id="div_grupoPoblacion6" class="cssDivCasillas posotionAbsolute topLinea10 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Identificador='Desplazados']/@Valor = 'True'">
          <div  id="div_grupoPoblacion7" class="cssDivCasillas posotionAbsolute topLinea10_2 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Identificador='Carcelarios']/@Valor = 'True'">
          <div  id="div_grupoPoblacion8" class="cssDivCasillas posotionAbsolute topLinea10_2 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Identificador='Indigentes']/@Valor = 'True'">
          <div  id="div_grupoPoblacion9" class="cssDivCasillas posotionAbsolute topLinea10_2 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Identificador='MadresComunitarias']/@Valor = 'True'">
          <div  id="div_grupoPoblacion10" class="cssDivCasillas posotionAbsolute topLinea10_2 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Identificador='CentrosP']/@Valor = 'True'">
          <div  id="div_grupoPoblacion11" class="cssDivCasillas posotionAbsolute topLinea10_2 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Identificador='Otros']/@Valor = 'True'">
          <div  id="div_grupoPoblacion12" class="cssDivCasillas posotionAbsolute topLinea10_2 " >
            X
          </div>
        </xsl:if>

        <!-- LINEA 10 FIN-->

        <!-- LINEA 11 -->
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Fuente Notificacion']/@Identificador = 'radioFuenteNotificacionR'">
          <div  id="div_fuente1" class="cssDivCasillas posotionAbsolute topLinea11 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Fuente Notificacion']/@Identificador = 'radioFuenteBAC'">
          <div  id="div_fuente2" class="cssDivCasillas posotionAbsolute topLinea11 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Fuente Notificacion']/@Identificador = 'radioFuenteBusquedaA'">
          <div  id="div_fuente3" class="cssDivCasillas posotionAbsolute topLinea11_2 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Fuente Notificacion']/@Identificador = 'radioFuenteInv'">
          <div  id="div_fuente4" class="cssDivCasillas posotionAbsolute topLinea11_2 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Fuente Notificacion']/@Identificador = 'radioFuenteVI'">
          <div  id="div_fuente5" class="cssDivCasillas posotionAbsolute topLinea11_3 " >
            X
          </div>
        </xsl:if>

        <div  id="div_paisNotificacionNombre" class="cssDivCasillas posotionAbsolute topLinea11_4" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputPais32']/@ValorTexto"/> - <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputDepartamento']/@ValorTexto"/>
        </div>
        <div  id="div_paisNotificacion" class="cssDivCasillas posotionAbsolute topLinea11_4" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputPais32']/@ValorTexto,1,3),3)"/>
        </div>
        <div  id="div_departamentoNotificacion" class="cssDivCasillas posotionAbsolute topLinea11_4" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputDepartamento']/@Valor,1,2),2)"/>
        </div>
        <div  id="div_municipioNotificacion" class="cssDivCasillas posotionAbsolute topLinea11_4" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputDepartamento']/@Valor,3,3),3)"/>
        </div>

        <!-- LINEA 11  FIN-->
        <!-- LINEA 12  -->
        <div  id="div_direccionResidencia" class="cssDivCasillas posotionAbsolute topLinea12 cssTextoMed" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Nombre='Dirección de residencia']/@Valor"/>
        </div>
        <!-- Campo faltante: Semanas de gestación -->
        <div id="div_SemGestacion" class="cssDivCasillas posotionAbsolute topLinea12" style="left: 900px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputSemGestacion']/@Valor"/>
        </div>
        <!-- LINEA 12  FIN-->
        <!-- LINEA 13  -->




        <div  id="div_fechaConsulta" class="cssDivCasillas posotionAbsolute topLinea13" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaConsulta']/@Valor,9,4), 4)"/>
        </div>

        <div  id="div_fechaConsulta1" class="cssDivCasillas posotionAbsolute topLinea13" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaConsulta']/@Valor,6,2), 2)"/>
        </div>

        <div  id="div_fechaConsulta2" class="cssDivCasillas posotionAbsolute topLinea13" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaConsulta']/@Valor,1,4), 8)"/>
        </div>

          
        <div  id="div_fechaInicioSintomas" class="cssDivCasillas posotionAbsolute topLinea13" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaSintomas']/@Valor,9,4), 4)"/>
        </div>

        <div  id="div_fechaInicioSintomas1" class="cssDivCasillas posotionAbsolute topLinea13" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaSintomas']/@Valor,6,2), 2)"/>
        </div>

        <div  id="div_fechaInicioSintomas2" class="cssDivCasillas posotionAbsolute topLinea13" >
          <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaSintomas']/@Valor,1,4), 8)"/>
        </div>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Clasificación inicial de caso']/@Identificador = 'Sospechoso'">
          <div  id="div_clasificacionInicial" class="cssDivCasillas posotionAbsolute topLinea13_1 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Clasificación inicial de caso']/@Identificador = 'Confporlaboratorio'">
          <div  id="div_clasificacionInicial3" class="cssDivCasillas posotionAbsolute topLinea13_1 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Clasificación inicial de caso']/@Identificador = 'Probable'">
          <div  id="div_clasificacionInicial2" class="cssDivCasillas posotionAbsolute topLinea13_2 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Clasificación inicial de caso']/@Identificador = 'ConfClínica'">
          <div  id="div_clasificacionInicial4" class="cssDivCasillas posotionAbsolute topLinea13_2 " >
            X
          </div>
        </xsl:if>
        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Clasificación inicial de caso']/@Identificador = 'Confnexoepidemiológico'">
          <div  id="div_clasificacionInicial5" class="cssDivCasillas posotionAbsolute topLinea13_3 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Hospitalizado']/@Identificador = 'hospSI'">
          <div  id="div_hospitalizado" class="cssDivCasillas posotionAbsolute topLinea13_4 " >
            X
          </div>
          <div  id="div_fechaHospitalizacion" class="cssDivCasillas posotionAbsolute topLinea14" >
            <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaConsultaNotificacion']/@Valor,1,2),2)"/>
          </div>

          <div  id="div_fechaHospitalizacion1" class="cssDivCasillas posotionAbsolute topLinea14" >
            <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaConsultaNotificacion']/@Valor,6,2),2)"/>
          </div>

          <div  id="div_fechaHospitalizacion2" class="cssDivCasillas posotionAbsolute topLinea14" >
            <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaConsultaNotificacion']/@Valor,1,4),8)"/>
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Hospitalizado']/@Identificador = 'hospNO'">
          <div  id="div_hospitalizadoNO" class="cssDivCasillas posotionAbsolute topLinea13_5 " >
            X
          </div>
        </xsl:if>

        <!-- LINEA 13  FIN-->

        <!-- LINEA 14  -->

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Condición final']/@Identificador = 'Vivo'">
          <div  id="div_CondicionFinal1" class="cssDivCasillas posotionAbsolute topLinea14_2 " >
            X
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Condición final']/@Identificador = 'Muerto'">
          <div  id="div_CondicionFinal2" class="cssDivCasillas posotionAbsolute topLinea14_3 " >
            X
          </div>
          <div  id="div_fechaDefuncion" class="cssDivCasillas posotionAbsolute topLinea14_3" >
            <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaDefuncion']/@Valor,1,2),2)"/>
          </div>

          <div  id="div_fechaDefuncion2" class="cssDivCasillas posotionAbsolute topLinea14_3" >
            <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaDefuncion']/@Valor,4,2),2)"/>
          </div>

          <div  id="div_fechaDefuncion3" class="cssDivCasillas posotionAbsolute topLinea14_3" >
            <xsl:value-of select="user:crearCeldas(substring(Bloques/Secciones/Campos[@Identificador='inputFechaDefuncion']/@Valor,7,4),4)"/>
          </div>

          <div  id="div_numeroCertificado" class="cssDivCasillas posotionAbsolute topLinea14_3 cssTextoDoc" >
            <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputNumeroCertificado']/@Valor"/>
          </div>


          <div  id="div_causaBasicaMuerte" class="cssDivCasillas  topLinea15_2 cssTextoPeq" >
            <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputCausaBasicaMuerte']/@Valor"/>
          </div>
        </xsl:if>

        <xsl:if test="Bloques/Secciones/Campos[@Nombre='Condición final']/@Identificador = 'NoSabe'">
          <div  id="div_CondicionFinal3" class="cssDivCasillas posotionAbsolute topLinea14_4 " >
            X
          </div>
        </xsl:if>
        <!-- LINEA 14 div_causaBasicaMuerte FIN-->
        <!-- LINEA 15  -->
        <div  id="div_nombreProfesional" class="cssDivCasillas posotionAbsolute topLinea15 cssTextoMed" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputProfesionalDiligencia']/@Valor"/>
        </div>
        <div  id="div_telefonoProfesional" class="cssDivCasillas posotionAbsolute topLinea15 cssTextoMed" >
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='inputTelefonoPersonal']/@Valor"/>
        </div>
        <!-- LINEA 15  FIN-->
        <!-- INICIO PÁGINA 2: Ítems 5 al 8 -->
        <div style="width: 100%;height:1200px;background-color: white; position: relative; page-break-before:always; background-image: url('#UrlDelSitio#/Registros/MortalidadMaterna/550_Mortalidad_Materna 2.jpg'); background-repeat: no-repeat; background-size: 100% 100%;" id="div_table_Parte_B">&nbsp;</div>
        <!-- Ítem 5: ANTECEDENTES MATERNOS -->
        <div id="div_AntecedentesMaternales" class="cssDivCasillas posotionAbsolute topLinea5" style="left: 50px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='Fiebre']/@Valor"/> <!-- Gestaciones -->
        </div>
        <div id="div_PartosVaginales" class="cssDivCasillas posotionAbsolute topLinea5" style="left: 200px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='Cefalea']/@Valor"/> <!-- Partos vaginales -->
        </div>
        <div id="div_Cesareas" class="cssDivCasillas posotionAbsolute topLinea5" style="left: 350px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='Ictericia']/@Valor"/> <!-- Cesáreas -->
        </div>
        <div id="div_Abortos" class="cssDivCasillas posotionAbsolute topLinea5" style="left: 500px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='Ictericia']/@Valor"/> <!-- Abortos -->
        </div>
        <div id="div_Muertos" class="cssDivCasillas posotionAbsolute topLinea5" style="left: 650px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='Mialgias']/@Valor"/> <!-- Muertos -->
        </div>
        <div id="div_Vivos" class="cssDivCasillas posotionAbsolute topLinea5" style="left: 800px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='Hepatomegalia']/@Valor"/> <!-- Vivos -->
        </div>
        <div id="div_RegulacionFecundidad" class="cssDivCasillas posotionAbsolute topLinea5_1" style="left: 50px; width: 300px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='OtraRegulacion']/@Valor"/> <!-- Regulación de la fecundidad -->
        </div>
        <!-- Ítem 6: ANTECEDENTES PRENATALES DEL EMBARAZO ACTUAL -->
        <div id="div_ControlPrenatalSi" class="cssDivCasillas posotionAbsolute topLinea6" style="left: 50px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='controlprenatalSi']/@Valor"/> <!-- Control prenatal Si -->
        </div>
        <div id="div_ControlPrenatalNo" class="cssDivCasillas posotionAbsolute topLinea6" style="left: 150px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='controlprenatalNo']/@Valor"/> <!-- Control prenatal No -->
        </div>
        <div id="div_NumeroCPN" class="cssDivCasillas posotionAbsolute topLinea6" style="left: 250px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='numeroCPN']/@Valor"/> <!-- Número C.P.N -->
        </div>
        <div id="div_SemanaInicioCPN" class="cssDivCasillas posotionAbsolute topLinea6" style="left: 350px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='semanaInicioCPN']/@Valor"/> <!-- Semana gestacional al inicio de C.P.N -->
        </div>
        <!-- Ítem 7: DATOS RELACIONADOS CON LA MUERTE MATERNA -->
        <div id="div_MomentoMuerte1" class="cssDivCasillas posotionAbsolute topLinea7" style="left: 50px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='momentoMuerte1']/@Valor"/> <!-- Durante la gestación -->
        </div>
        <div id="div_MomentoMuerte5" class="cssDivCasillas posotionAbsolute topLinea7" style="left: 200px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='momentoMuerte5']/@Valor"/> <!-- 42 días postparto -->
        </div>
        <div id="div_MomentoMuerte6" class="cssDivCasillas posotionAbsolute topLinea7" style="left: 350px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='momentoMuerte6']/@Valor"/> <!-- 43 días a un año postparto -->
        </div>
        <div id="div_FechaTerminacionGestacion" class="cssDivCasillas posotionAbsolute topLinea7_1" style="left: 500px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='fechaTerminacionGestacion']/@Valor"/> <!-- Fecha de terminación de la gestación -->
        </div>
        <div id="div_TipoParto1" class="cssDivCasillas posotionAbsolute topLinea7_2" style="left: 50px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='tipoParto1']/@Valor"/> <!-- Vaginal -->
        </div>
        <div id="div_TipoParto2" class="cssDivCasillas posotionAbsolute topLinea7_2" style="left: 150px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='tipoParto2']/@Valor"/> <!-- Cesárea -->
        </div>
        <div id="div_TipoParto3" class="cssDivCasillas posotionAbsolute topLinea7_2" style="left: 250px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='tipoParto3']/@Valor"/> <!-- Instrumentado -->
        </div>
        <div id="div_TipoParto4" class="cssDivCasillas posotionAbsolute topLinea7_2" style="left: 350px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='tipoParto4']/@Valor"/> <!-- Ignorado -->
        </div>
        <div id="div_TipoParto5" class="cssDivCasillas posotionAbsolute topLinea7_2" style="left: 450px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='tipoParto5']/@Valor"/> <!-- No nació -->
        </div>
        <div id="div_PartoAtendidoPor1" class="cssDivCasillas posotionAbsolute topLinea7_3" style="left: 50px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='partoAtendidoPor1']/@Valor"/> <!-- Médico general -->
        </div>
        <div id="div_PartoAtendidoPor2" class="cssDivCasillas posotionAbsolute topLinea7_3" style="left: 150px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='partoAtendidoPor2']/@Valor"/> <!-- Obstetra -->
        </div>
        <div id="div_PartoAtendidoPor3" class="cssDivCasillas posotionAbsolute topLinea7_3" style="left: 250px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='partoAtendidoPor3']/@Valor"/> <!-- Enfermera -->
        </div>
        <div id="div_PartoAtendidoPor4" class="cssDivCasillas posotionAbsolute topLinea7_3" style="left: 350px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='partoAtendidoPor4']/@Valor"/> <!-- Auxiliar -->
        </div>
        <div id="div_PartoAtendidoPor5" class="cssDivCasillas posotionAbsolute topLinea7_3" style="left: 450px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='partoAtendidoPor5']/@Valor"/> <!-- Promotor -->
        </div>
        <div id="div_PartoAtendidoPor6" class="cssDivCasillas posotionAbsolute topLinea7_3" style="left: 550px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='partoAtendidoPor6']/@Valor"/> <!-- Partera -->
        </div>
        <div id="div_PartoAtendidoPor8" class="cssDivCasillas posotionAbsolute topLinea7_3" style="left: 650px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='partoAtendidoPor8']/@Valor"/> <!-- Ella misma -->
        </div>
        <div id="div_PartoAtendidoPor7" class="cssDivCasillas posotionAbsolute topLinea7_3" style="left: 750px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='partoAtendidoPor7']/@Valor"/> <!-- Otro ¿Cuál? -->
        </div>
        <div id="div_SitioMuerte6" class="cssDivCasillas posotionAbsolute topLinea7_4" style="left: 50px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='sitioMuerte6']/@Valor"/> <!-- Domicilio -->
        </div>
        <div id="div_SitioMuerte8" class="cssDivCasillas posotionAbsolute topLinea7_4" style="left: 200px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='sitioMuerte8']/@Valor"/> <!-- Institucional -->
        </div>
        <div id="div_SitioMuerte7" class="cssDivCasillas posotionAbsolute topLinea7_4" style="left: 350px; width: 120px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='sitioMuerte7']/@Valor"/> <!-- Otro -->
        </div>
        <!-- Ítem 8: DATOS DE LA PERSONA MÁS CERCANA DE LA MUJER FALLECIDA -->
        <div id="div_NombrePersonaCercana" class="cssDivCasillas posotionAbsolute topLinea8" style="left: 50px; width: 200px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='nombrePersonaCercana']/@Valor"/> <!-- Nombres y apellidos -->
        </div>
        <div id="div_PaisPersonaCercana" class="cssDivCasillas posotionAbsolute topLinea8" style="left: 270px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='paisPersonaCercana']/@Valor"/> <!-- País -->
        </div>
        <div id="div_DepartamentoPersonaCercana" class="cssDivCasillas posotionAbsolute topLinea8" style="left: 390px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='departamentoPersonaCercana']/@Valor"/> <!-- Departamento -->
        </div>
        <div id="div_MunicipioPersonaCercana" class="cssDivCasillas posotionAbsolute topLinea8" style="left: 510px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='municipioPersonaCercana']/@Valor"/> <!-- Municipio -->
        </div>
        <div id="div_LocalidadPersonaCercana" class="cssDivCasillas posotionAbsolute topLinea8_2" style="left: 50px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='localidadPersonaCercana']/@Valor"/> <!-- Localidad -->
        </div>
        <div id="div_BarrioPersonaCercana" class="cssDivCasillas posotionAbsolute topLinea8_2" style="left: 170px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='barrioPersonaCercana']/@Valor"/> <!-- Barrio -->
        </div>
        <div id="div_CabeceraPersonaCercana" class="cssDivCasillas posotionAbsolute topLinea8_2" style="left: 290px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='cabeceraPersonaCercana']/@Valor"/> <!-- Cabecera municipal/centro poblado/rural disperso -->
        </div>
        <div id="div_VeredaPersonaCercana" class="cssDivCasillas posotionAbsolute topLinea8_2" style="left: 410px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='veredaPersonaCercana']/@Valor"/> <!-- Vereda/zona -->
        </div>
        <div id="div_DireccionPersonaCercana" class="cssDivCasillas posotionAbsolute topLinea8_3" style="left: 50px; width: 200px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='direccionPersonaCercana']/@Valor"/> <!-- Dirección -->
        </div>
        <div id="div_TelefonoPersonaCercana" class="cssDivCasillas posotionAbsolute topLinea8_3" style="left: 270px; width: 100px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='telefonoPersonaCercana']/@Valor"/> <!-- Teléfono -->
        </div>
        <div id="div_ParentescoPadre" class="cssDivCasillas posotionAbsolute topLinea8_4" style="left: 50px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='parentesco1']/@Valor"/> <!-- Padre -->
        </div>
        <div id="div_ParentescoMadre" class="cssDivCasillas posotionAbsolute topLinea8_4" style="left: 150px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='parentesco2']/@Valor"/> <!-- Madre -->
        </div>
        <div id="div_ParentescoHermano" class="cssDivCasillas posotionAbsolute topLinea8_4" style="left: 250px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='parentesco3']/@Valor"/> <!-- Hermano(a) -->
        </div>
        <div id="div_ParentescoEsposo" class="cssDivCasillas posotionAbsolute topLinea8_4" style="left: 350px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='parentesco4']/@Valor"/> <!-- Esposo(a) -->
        </div>
        <div id="div_ParentescoHijo" class="cssDivCasillas posotionAbsolute topLinea8_4" style="left: 450px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='parentesco5']/@Valor"/> <!-- Hijo(a) -->
        </div>
        <div id="div_ParentescoAmigo" class="cssDivCasillas posotionAbsolute topLinea8_4" style="left: 550px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='parentesco6']/@Valor"/> <!-- Amigo(a) -->
        </div>
        <div id="div_ParentescoOtro" class="cssDivCasillas posotionAbsolute topLinea8_4" style="left: 650px; width: 80px;">
          <xsl:value-of select="Bloques/Secciones/Campos[@Identificador='parentescoOtro']/@Valor"/> <!-- Otro -->
        </div>


       
      </body>



    </html>
  </xsl:template>
</xsl:stylesheet>

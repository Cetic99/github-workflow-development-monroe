/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import CircleButton from '@ui/components/circle-button'
import IconEnglishFlag from '@ui/components/icons/IconEnglishFlag'
import IconSerbianFlag from '@ui/components/icons/IconSerbianFlag'
import { useCurrentLanguage } from '@domain/administration/stores'
import { Language } from '@domain/administration/constants'
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  align-items: center;
`

const LanguageSwitcherButton = ({ onClick = () => {} }) => {
  const language = useCurrentLanguage()
  return (
    <Container>
      {Language.EN === language && (
        <CircleButton
          color="transparent"
          size="s"
          icon={(props) => <IconEnglishFlag {...props} />}
          onClick={() => onClick()}
        />
      )}

      {Language.BS === language && (
        <CircleButton
          color="transparent"
          size="s"
          icon={(props) => <IconSerbianFlag {...props} />}
          onClick={() => onClick()}
        />
      )}
    </Container>
  )
}

export default LanguageSwitcherButton
